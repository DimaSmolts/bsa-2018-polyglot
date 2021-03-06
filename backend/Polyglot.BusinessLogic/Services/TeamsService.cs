﻿using AutoMapper;
using Polyglot.BusinessLogic.Interfaces;
using Polyglot.Common.DTOs;
using Polyglot.DataAccess.Entities;
using Polyglot.DataAccess.Helpers;
using Polyglot.DataAccess.SqlRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polyglot.Core.Authentication;
using Polyglot.DataAccess.Entities.Chat;
using Polyglot.DataAccess.MongoRepository;
using System;

namespace Polyglot.BusinessLogic.Services
{
    public class TeamsService : CRUDService<Team, TeamDTO>, ITeamService
    {
        INotificationService notificationService;
		    private readonly IMongoRepository<DataAccess.MongoModels.ComplexString> stringsProvider;
        private readonly ICurrentUser _currentUser;

        public TeamsService(IUnitOfWork uow, IMapper mapper, INotificationService notificationService,
		                    IMongoRepository<DataAccess.MongoModels.ComplexString> rep, ICurrentUser currentUser)
            :base(uow, mapper)

        {
			      this.stringsProvider = rep;
            _currentUser = currentUser;
            this.notificationService = notificationService;
        }

		public async Task<List<TeamPrevDTO>> SearchTeams(string query)
		{
			IEnumerable<TeamPrevDTO> all = await GetAllTeamsAsync();

			List<TeamPrevDTO> selected = new List<TeamPrevDTO>();

			foreach(var t in all)
			{
				if(t.Name.Contains(query))
				{
					selected.Add(t);
				}
			}
			return selected;
		}

        public async Task<IEnumerable<TeamPrevDTO>> GetAllTeamsAsync()
        {
            var user = await _currentUser.GetCurrentUserProfile();
            IEnumerable<Team> result = new List<Team>();

			if (user.UserRole == Role.Translator)
			{
				var translatorTeams = await uow.GetRepository<TeamTranslator>().GetAllAsync(x => x.TranslatorId == user.Id);
				var allTeams = await uow.GetRepository<Team>().GetAllAsync();
				result = allTeams.Where(x => translatorTeams.Any(y => y.TeamId == x.Id));
			}
			else
			{
				result = await uow.GetRepository<Team>().GetAllAsync(x => x.CreatedBy == user);
			}

			return mapper.Map<IEnumerable<TeamPrevDTO>>(result);
		}

        public async Task<TeamDTO> FormTeamAsync(ReceiveTeamDTO receivedTeam)
        {
            var currentUser = await _currentUser.GetCurrentUserProfile();
            if (currentUser == null || currentUser.UserRole != Role.Manager)
                return null;

			var userRepo = uow.GetRepository<UserProfile>();

			List<TeamTranslator> translators = new List<TeamTranslator>();
			UserProfile currentTranslator;
			List<DialogParticipant> teamChatDialogParticipants = new List<DialogParticipant>()
			{
				new DialogParticipant() { Participant = currentUser }
			};

			foreach (var id in receivedTeam.TranslatorIds)
			{
				currentTranslator = await userRepo.GetAsync(id);
				if (currentTranslator != null && currentTranslator.UserRole == Role.Translator)
				{
					translators.Add(new TeamTranslator
					{
						UserProfile = currentTranslator
					});

					if (currentTranslator.Id != currentUser.Id)
						teamChatDialogParticipants.Add(new DialogParticipant() { Participant = currentTranslator });
				}
			}

			if (translators.Count < 1)
				return null;

            Team newTeam = await uow.GetRepository<Team>().CreateAsync(
                    new Team()
                    {
                        TeamTranslators = translators,
                        CreatedBy = currentUser,
                        Name = receivedTeam.Name
                    });


            await uow.SaveAsync();
            foreach (var translator in newTeam.TeamTranslators)
            {
                await notificationService.SendNotification(new NotificationDTO
                {
                    SenderId = currentUser.Id,
                    Message = $"You received an invitation in team {newTeam.Name}",
                    ReceiverId = translator.TranslatorId,
                    NotificationAction = NotificationAction.JoinTeam,
                    Payload = newTeam.Id,
                    Options = new List<OptionDTO>()
                        {
                            new OptionDTO()
                            {
                                OptionDefinition = OptionDefinition.Accept
                            },
                            new OptionDTO()
                            {
                                OptionDefinition = OptionDefinition.Decline
                            }
                        }
                });
            }

			var teamChatDialog = new ChatDialog()
			{
				DialogName = newTeam.Name,
				DialogParticipants = teamChatDialogParticipants,
				DialogType = ChatGroup.chatTeam,
				Identifier = newTeam.Id
			};
			await uow.GetRepository<ChatDialog>().CreateAsync(teamChatDialog);
			await uow.SaveAsync();

			return newTeam != null ? mapper.Map<TeamDTO>(newTeam) : null;
		}

		public async Task<bool> TryDisbandTeamAsync(int teamId)
		{
			await uow.GetRepository<Team>().DeleteAsync(teamId);
			var success = await uow.SaveAsync() > 0;

			if (success)
			{
				var targetTeamDialog = uow.GetRepository<ChatDialog>()
				.GetAsync(d => d.DialogType == ChatGroup.chatTeam && d.Identifier == teamId)
				?.Id;
				if (targetTeamDialog.HasValue)
				{
					await uow.GetRepository<ChatDialog>().DeleteAsync(targetTeamDialog.Value);
				}
				await uow.SaveAsync();
			}
			return success;
		}


        #region Overrides


		public override async Task<TeamDTO> GetOneAsync(int teamId)
		{
			var team = await uow.GetRepository<Team>().GetAsync(teamId);

            if (team?.TeamTranslators?.Any() != true)
                return mapper.Map<TeamDTO>(team);


			var translators = team.TeamTranslators;
			// вычисляем рейтинги переводчиков
			Dictionary<int, double> ratings = new Dictionary<int, double>();
			IEnumerable<double> ratingRatesSequence;
			double? currentRating;

			foreach (var t in translators)
			{
				ratingRatesSequence = t.UserProfile?.Ratings.Select(r => r.Rate);
				if (ratingRatesSequence.Count() < 1)
					currentRating = 0.0d;
				else
					currentRating = ratingRatesSequence.Average();

				ratings.Add(t.UserProfile.Id, currentRating.HasValue ? currentRating.Value : 0.0d);
			}
			//маппим и после маппинга заполняем рейтинг
			var teamTranslators = mapper.Map<IEnumerable<TeamTranslator>, IEnumerable<TranslatorDTO>>(translators, opt => opt.AfterMap((src, dest) =>
			{
				var translatorsList = dest.ToList();
				for (int i = 0; i < translatorsList.Count; i++)
				{
					translatorsList[i].Rating = ratings[translatorsList[i].UserId];
				}

			}));

			var teamsProjects = mapper.Map<IEnumerable<TeamProjectDTO>>(team.ProjectTeams);

			foreach(var p in teamsProjects)
			{				
				var targetProject = await uow.GetRepository<Project>().GetAsync(p.ProjectId);
				List<DataAccess.MongoModels.ComplexString> temp = new List<DataAccess.MongoModels.ComplexString>();
				temp = await stringsProvider.GetAllAsync(str => str.ProjectId == targetProject.Id);
				int languagesAmount = targetProject.ProjectLanguageses.Count;
				int max = temp.Count * languagesAmount;
				int currentProgress = 0;
				foreach (var str in temp)
				{
					currentProgress += str.Translations.Count;
				}
				if (currentProgress == 0 || max == 0)
				{
					p.Progress = 0;
				}
				else
				{
					p.Progress = Convert.ToInt32((Convert.ToDouble(currentProgress) / Convert.ToDouble(max)) * 100);
				}
			}

			return new TeamDTO()
			{
				Id = team.Id,
				Name = team.Name,
				TeamTranslators = teamTranslators.ToList(),
				TeamProjects = teamsProjects.ToList()
			};

		}

		public override async Task<TeamDTO> PutAsync(TeamDTO entity)
		{
			if (uow != null)
			{
				var teamRepository = uow.GetRepository<Team>();
				var target = await teamRepository.Update(mapper.Map<Team>(entity));
				if (target != null)
				{
					await uow.SaveAsync();
					return mapper.Map<TeamDTO>(target);
				}
			}
			return null;
		}

		#endregion Overrides

		#region Translators


		public async Task<IEnumerable<TranslatorDTO>> GetAllTranslatorsAsync()
		{
			var translators = await uow.GetRepository<UserProfile>()
				.GetAllAsync(u => u.UserRole == Role.Translator);

			if (translators != null && translators.Count > 0)
			{

				var tLanguages = await uow.GetMidRepository<TranslatorLanguage>()
					.GetAllAsync();
				// вычисляем рейтинги переводчиков
				Dictionary<int, double> ratings = new Dictionary<int, double>();
				IEnumerable<double> ratingRatesSequence;
				double? currentRating;

				foreach (var t in translators)
				{
					ratingRatesSequence = t.Ratings.Select(r => r.Rate);
					if (ratingRatesSequence.Count() < 1)
						currentRating = 0.0d;
					else
						currentRating = ratingRatesSequence.Average();

					ratings.Add(t.Id, currentRating.HasValue ? currentRating.Value : 0.0d);
				}
				//маппим и после маппинга заполняем рейтинг
				return mapper.Map<IEnumerable<UserProfile>, IEnumerable<TranslatorDTO>>(translators, opt => opt.AfterMap((src, dest) =>
				{
					var translatorsList = dest.ToList();
					IEnumerable<TranslatorLanguage> translatorLanguages;
					for (int i = 0; i < translatorsList.Count; i++)
					{
						translatorsList[i].Rating = ratings[translatorsList[i].UserId];
						// добавляем инфо о языках
						translatorLanguages = tLanguages.Where(tl => tl.TranslatorId == translatorsList[i].UserId);
						if (translatorLanguages != null && translatorLanguages.Count() > 0)
							translatorsList[i].TranslatorLanguages = mapper.Map<IEnumerable<TranslatorLanguageDTO>>(translatorLanguages);
					}

				}));
			}
			else
				return new List<TranslatorDTO>();
		}

		public async Task<IEnumerable<TranslatorDTO>> GetFilteredtranslators(int prof, int[] languages)
		{
			IEnumerable<TranslatorDTO> entities = await this.GetAllTranslatorsAsync();
			if (languages.Count() == 0 && prof == 0)
				return entities;

			List<TranslatorDTO> result = new List<TranslatorDTO>();
			
			if (languages.Count() == 0)
			{
				foreach (var entity in entities)
				{
					foreach (var lang in entity.TranslatorLanguages)
					{
						if ((int)lang.Proficiency >= prof)
						{
							result.Add(entity);
							break;
						}
					}
				}
			}
			else
			{
				foreach (var entity in entities)
				{
					foreach (var lang in entity.TranslatorLanguages)
					{
						if ((int)lang.Proficiency >= prof && languages.Contains(lang.Language.Id))
						{
							result.Add(entity);
							break;
						}
					}
				}
			}
			return result;
		}


		public async Task<TranslatorDTO> GetTranslatorAysnc(int id)
		{
			var translator = await uow.GetRepository<UserProfile>().GetAsync(id);
			if (translator != null && translator.UserRole == Role.Translator)
			{
				var translatorLanguages = await uow.GetMidRepository<TranslatorLanguage>()
					.GetAllAsync(tl => tl.TranslatorId == translator.Id);

				return mapper.Map<UserProfile, TranslatorDTO>(translator, opt => opt.AfterMap((src, dest) =>
				{
					var ratingRatesSequence = src.Ratings.Select(r => r.Rate);
					if (ratingRatesSequence.Count() < 1)
						dest.Rating = 0.0d;
					else
						dest.Rating = src.Ratings.Select(r => r.Rate).Average();

					// добавляем инфо о языках
					if (translatorLanguages != null && translatorLanguages.Count() > 0)
						dest.TranslatorLanguages = mapper.Map<IEnumerable<TranslatorLanguageDTO>>(translatorLanguages);
				}));
			}
			else
				return null;
		}

		public async Task<double> GetTranslatorRatingValueAsync(int translatorId)
		{
			var translator = await uow.GetRepository<UserProfile>().GetAsync(translatorId);
			if (translator != null && translator.UserRole == Role.Translator)
			{
				var ratingRatesSequence = translator.Ratings.Select(r => r.Rate);
				if (ratingRatesSequence.Count() > 0)
					return ratingRatesSequence.Average();
			}

			return 0.0d;
		}

        public async Task<TranslatorDTO> ActivateUserInTeam(int userId, int teamId)
        {
            var teamTranslator = await uow.GetRepository<TeamTranslator>().GetAsync(t => t.TranslatorId == userId && t.TeamId == teamId);
            if (teamTranslator != null) {
                teamTranslator.IsActivated = true;
                teamTranslator = await uow.GetRepository<TeamTranslator>().Update(teamTranslator);
                await uow.SaveAsync();
                return mapper.Map<TranslatorDTO>(teamTranslator);
            }
            return new TranslatorDTO() { };
        }


		public async Task<TeamDTO> TryAddTeamAsync(TeamTranslatorsDTO teamTranslators)
		{

            foreach (var translatorId in teamTranslators.TranslatorIds)
            {
                var newTeamTranslator = new TeamTranslator
                {
                    TeamId = teamTranslators.TeamId,
                    TranslatorId = translatorId

                };

                var teamTranslator = await uow.GetRepository<TeamTranslator>().CreateAsync(newTeamTranslator);

                await notificationService.SendNotification(new NotificationDTO
                {
                    SenderId = translatorId,
                    Message = $"You received an invitation in team {teamTranslators.TeamName}",
                    ReceiverId = translatorId,
                    NotificationAction = NotificationAction.JoinTeam,
                    Payload = teamTranslators.TeamId,
                    Options = new List<OptionDTO>()
                        {
                            new OptionDTO()
                            {
                                OptionDefinition = OptionDefinition.Accept
                            },
                            new OptionDTO()
                            {
                                OptionDefinition = OptionDefinition.Decline
                            }
                        }
                });

            }
            await uow.SaveAsync();
            var team = await uow.GetRepository<Team>().GetAsync(teamTranslators.TeamId);
            return mapper.Map<TeamDTO>(team);
        }


        public async Task<TeamDTO> DeleteUserFromTeam(int userId, int teamId)
        {
            var translator = await uow.GetRepository<TeamTranslator>().GetAsync(t => t.TranslatorId == userId && t.TeamId == teamId);
            var deletedTranslator = uow.GetRepository<TeamTranslator>().DeleteAsync(translator.Id);
            var team = await uow.GetRepository<Team>().GetAsync(translator.TeamId);
            await uow.SaveAsync();

            return mapper.Map<TeamDTO>(team);
        }
        #endregion Translators
    }
}
