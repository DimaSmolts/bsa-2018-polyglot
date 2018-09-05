﻿using AutoMapper;
using Polyglot.BusinessLogic.Interfaces;
using Polyglot.Common.DTOs;
using Polyglot.Common.DTOs.Chat;
using Polyglot.Common.Helpers;
using Polyglot.Core.Authentication;
using Polyglot.DataAccess.Entities;
using Polyglot.DataAccess.Helpers;
using Polyglot.DataAccess.SqlRepository;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polyglot.BusinessLogic.Services.SignalR;
using Polyglot.BusinessLogic.Interfaces.SignalR;
using Microsoft.AspNetCore.Authorization;
using Polyglot.DataAccess.Entities.Chat;

namespace Polyglot.BusinessLogic.Services
{
    [Authorize]
    public class ChatService : IChatService
    {
        private readonly IProjectService projectService;
        private readonly ITeamService teamService;
        private readonly ISignalRChatService signalRChatService;
        private readonly IMapper mapper;
        private readonly IUnitOfWork uow;

        public ChatService(
            ISignalRChatService signalRChatService,
            IProjectService projectService,
            ITeamService teamService, IMapper mapper, IUnitOfWork uow)
        {
            this.signalRChatService = signalRChatService;
            this.projectService = projectService;
            this.teamService = teamService;
            this.mapper = mapper;
            this.uow = uow;
        }



        public async Task<IEnumerable<ChatDialogDTO>> GetDialogsAsync(ChatGroup targetGroup)
        {
            var currentUser = await CurrentUser.GetCurrentUserProfile();
            if (currentUser == null)
                return null;

            var dialogs = await uow.GetRepository<ChatDialog>()
                .GetAllAsync(d => d.DialogType == targetGroup && d.DialogParticipants.Select(dp => dp.Participant).Contains(currentUser));

            if (dialogs == null || dialogs.Count < 1)
                return null;

            List<ChatDialogDTO> result = null;

            switch (targetGroup)
            {
                case ChatGroup.direct:
                case ChatGroup.dialog:
                    result = mapper.Map<List<ChatDialog>, List<ChatDialogDTO>>(dialogs, opt => opt.AfterMap((src, dest) => FormUserDialogs(src, dest, currentUser)));
                    break;
                case ChatGroup.chatProject:
                case ChatGroup.chatTeam:
#warning добавить participants
                    result = mapper.Map<List<ChatDialog>, List<ChatDialogDTO>>(dialogs);
                    break;
                default:
                    break;
            }

            return result;
        }

        public void FormUserDialogs(List<ChatDialog> src, List<ChatDialogDTO> dest, UserProfile currentUser)
        {
            List<ChatDialogDTO> result = dest;
            result.ForEach(d =>
            {
                var accordingSourceDialog = src.Find(dialog => dialog.Id == d.Id);
                var interlocator = accordingSourceDialog
                    .DialogParticipants
                    .Select(dp => dp.Participant)
                    .Where(p => p.Id != currentUser.Id)
                    .FirstOrDefault();

                if (interlocator != null)
                {
                    d.LastMessageText = accordingSourceDialog
                        .Messages.LastOrDefault(m => m.SenderId == interlocator.Id)
                        ?.Body;

                    if (d.LastMessageText.Length > 155)
                    {
                        d.LastMessageText = d.LastMessageText.Substring(0, 150);
                    }

                    d.UnreadMessagesCount = accordingSourceDialog.Messages
                        .Where(m => m.SenderId == interlocator.Id && !m.IsRead)
                        .Count();

                    d.Participants = new List<ChatUserDTO>()
                    {
                        mapper.Map<ChatUserDTO>(interlocator)
                    };
                }
            });
        }

        public async Task<IEnumerable<ChatMessageDTO>> GetDialogMessagesAsync(ChatGroup targetGroup, int targetGroupDialogId)
        {
            if (targetGroupDialogId < 0)
                return null;

            var currentUser = await CurrentUser.GetCurrentUserProfile();
            if (currentUser == null)
                return null;

            IEnumerable<ChatMessage> messages = null;
            long dialogIdentifer = -1;

            switch (targetGroup)
            {
                case ChatGroup.dialog:
                case ChatGroup.direct:
                    {
                        dialogIdentifer = currentUser.Id + targetGroupDialogId;
                        break;
                    }
                case ChatGroup.chatProject:
                case ChatGroup.chatTeam:
                    {
                        dialogIdentifer = targetGroupDialogId;
                        break;
                    }
                default:
                    break;
            }

            messages = (await uow.GetRepository<ChatDialog>()
                .GetAsync(d => d.Identifier == dialogIdentifer && d.DialogType == targetGroup))
                ?.Messages.AsEnumerable();

            return messages != null ? mapper.Map<IEnumerable<ChatMessageDTO>>(messages) : null;
        }

        public async Task<ChatMessageDTO> SendMessage(ChatMessageDTO message)
        {
            var targetDialog = await uow.GetRepository<ChatDialog>().GetAsync(message.DialogId);
            if (targetDialog == null)
                return null;

            var currentUser = await CurrentUser.GetCurrentUserProfile();
            if (currentUser == null || message == null)
                return null;

            message.ReceivedDate = DateTime.Now;
            message.SenderId = currentUser.Id;
            var newMessage = await uow.GetRepository<ChatMessage>().CreateAsync(mapper.Map<ChatMessage>(message));
            await uow.SaveAsync();

            if (newMessage == null)
                return null;

            var dialogId = newMessage.DialogId.Value;
            var text = newMessage.Body;
            // отправляем уведомление о сообщении в диалог
            await signalRChatService.MessageReveived($"{targetDialog.DialogType.ToString()}{dialogId}", dialogId, newMessage.Id, text);

            //отправляем уведомление каждому участнику диалога
            foreach (var participant in targetDialog.DialogParticipants)
            {
                await signalRChatService.MessageReveived($"{ChatGroup.direct.ToString()}{participant.ParticipantId}", dialogId, newMessage.Id, text);
            }

            return mapper.Map<ChatMessageDTO>(newMessage);
        }

        //public async Task<ChatContactsDTO> GetDialogsAsync(ChatGroup targetGroup, int targetGroupItemId)
        //{
        //    var currentUser = await CurrentUser.GetCurrentUserProfile();
        //    if (currentUser == null)
        //        return null;

        //    ChatContactsDTO contacts = new ChatContactsDTO()
        //    {
        //        ChatUserId = targetGroupItemId
        //    };

        //    switch (targetGroup)
        //    {
        //        case ChatGroup.chatUser:
        //            {
        //                var c = await GetUserContacts(currentUser);
        //                if(c != null)
        //                {
        //                    contacts.ContactList = c;
        //                }
        //                break;
        //            }
        //        case ChatGroup.Project:
        //            break;
        //        case ChatGroup.Team:
        //            break;
        //        default:
        //            break;
        //    }
        //    return contacts;
        //}

        public async Task<IEnumerable<ChatUserStateDTO>> GetUsersStateAsync()
        {
            var currentUser = await CurrentUser.GetCurrentUserProfile();
            if (currentUser == null)
                return null;

            return null;
        }

        //public async Task<IEnumerable<ChatMessageDTO>> GetGroupMessagesHistoryAsync(ChatGroup targetGroup, int targetGroupItemId)
        //{
        //    IEnumerable<ChatMessage> messages = null;
        //    var currentUser = await CurrentUser.GetCurrentUserProfile();
        //    if (currentUser == null)
        //        return null;

        //    switch (targetGroup)
        //    {
        //        case ChatGroup.direct:
        //            {
        //                messages = await uow.GetRepository<ChatMessage>()
        //                    .GetAllAsync(m => 
        //                    (m.RecipientId == currentUser.Id && m.SenderId == targetGroupItemId) 
        //                    || 
        //                    (m.RecipientId == targetGroupItemId && m.SenderId == currentUser.Id));
        //                break;
        //            }
        //        case ChatGroup.Project:
        //            break;
        //        case ChatGroup.Team:
        //            break;
        //        default:
        //            break;
        //    }

        //    return mapper.Map<IEnumerable<ChatMessageDTO>>(messages);
        //}

        public async Task<ChatMessageDTO> GetMessageAsync(int messageId)
        {
            var currentUser = await CurrentUser.GetCurrentUserProfile();
            if (currentUser == null)
                return null;

            var message = await uow.GetRepository<ChatMessage>()
                            .GetAsync(messageId);

            if (message == null)
                return null;

            // current user должен принимать участие в диалоге к которому относится искомое сообщение 
            var targetDialog = message.Dialog
                ?.DialogParticipants
                ?.Select(dp => dp.Participant)
                ?.Where(p => p.Id == currentUser.Id)
                ?.FirstOrDefault();

            return targetDialog != null ? mapper.Map<ChatMessageDTO>(message) : null;
        }

        public Task<ChatUserStateDTO> GetUserStateAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjectDTO>> GetProjectsAsync()
        {
            return await projectService.GetListAsync() ?? null;
        }

        public async Task<IEnumerable<TeamPrevDTO>> GetTeamsAsync()
        {
            return await teamService.GetAllTeamsAsync() ?? null;
        }


        //public async Task<ChatMessageDTO> SendMessage(ChatMessageDTO message, ChatGroup targetGroup, int targetGroupItemId)
        //{
        //    var currentUser = await CurrentUser.GetCurrentUserProfile();
        //    if (currentUser == null || message == null)
        //        return null;

        //    ChatMessage addedMessage = null;

        //    switch (targetGroup)
        //    {
        //        case ChatGroup.direct:
        //            {
        //                message.IsRead = false;
        //                message.ReceivedDate = DateTime.Now;
        //                message.SenderId = currentUser.Id;
        //                addedMessage = await uow.GetRepository<ChatMessage>().CreateAsync(mapper.Map<ChatMessage>(message));
        //                await signalRChatService.MessageReveived($"{targetGroup}{targetGroupItemId}", message);
        //                break;
        //            }

        //        case ChatGroup.Project:
        //            break;
        //        case ChatGroup.Team:
        //            break;
        //        default:
        //            break;
        //    }

        //    return (await uow.SaveAsync() > 0 && addedMessage != null) ? mapper.Map<ChatMessageDTO>(addedMessage) : null;
        //}
    }
}


//messages = new List<ChatMessageDTO>
//{
//    new ChatMessageDTO()
//    {
//        Id = 0,
//        IsRead = true,
//        ReceivedDate = DateTime.Now,
//        RecipientId = currentUser.Id,
//        SenderId = targetGroupItemId,
//        Body = "My father, a good man, told me, 'Never lose your ignorance; you cannot replace it."
//    },
//    new ChatMessageDTO()
//    {
//        Id = 1,
//        IsRead = true,
//        ReceivedDate = DateTime.Now,
//        RecipientId = currentUser.Id,
//        SenderId = targetGroupItemId,
//        Body = "If truth is beauty, how come no one has their hair done in the library?"
//    },
//    new ChatMessageDTO()
//    {
//        Id = 2,
//        IsRead = true,
//        ReceivedDate = DateTime.Now,
//        RecipientId = targetGroupItemId,
//        SenderId = currentUser.Id,
//        Body = "My father, a good man, told me, 'Never lose your ignorance; you cannot replace it."
//    },
//    new ChatMessageDTO()
//    {
//        Id = 3,
//        IsRead = true,
//        ReceivedDate = DateTime.Now,
//        RecipientId = currentUser.Id,
//        SenderId = targetGroupItemId,
//        Body = "Ignorance must certainly be bliss or there wouldn't be so many people so resolutely pursuing it.My father, a good man, told me, 'Never lose your ignorance; you cannot replace it."
//    },
//    new ChatMessageDTO()
//    {
//        Id = 4,
//        IsRead = true,
//        ReceivedDate = DateTime.Now,
//        RecipientId = currentUser.Id,
//        SenderId = targetGroupItemId,
//        Body = "the high school after high school!"
//    },
//    new ChatMessageDTO()
//    {
//        Id = 5,
//        IsRead = true,
//        ReceivedDate = DateTime.Now,
//        RecipientId = targetGroupItemId,
//        SenderId = currentUser.Id,
//        Body = "A professor is one who talks in someone else's sleep."
//    },
//    new ChatMessageDTO()
//    {
//        Id = 6,
//        IsRead = true,
//        ReceivedDate = DateTime.Now,
//        RecipientId = targetGroupItemId,
//        SenderId = currentUser.Id,
//        Body = "About all some men accomplish in life is to send a son to Harvard. My father, a good man, told me, 'Never lose your ignorance; you cannot replace it."
//    },new ChatMessageDTO()
//    {
//        Id = 7,
//        IsRead = false,
//        ReceivedDate = DateTime.Now,
//        RecipientId = currentUser.Id,
//        SenderId = targetGroupItemId,
//        Body = "My father, a good man, told me, 'Never lose your ignorance; you cannot replace it."
//    },
//    new ChatMessageDTO()
//    {
//        Id = 8,
//        IsRead = false,
//        ReceivedDate = DateTime.Now,
//        RecipientId = targetGroupItemId,
//        SenderId = currentUser.Id,
//        Body = "The world is coming to an end! Repent and return those library books!"
//    },
//    new ChatMessageDTO()
//    {
//        Id = 9,
//        IsRead = false,
//        ReceivedDate = DateTime.Now,
//        RecipientId = targetGroupItemId,
//        SenderId = currentUser.Id,
//        Body = "So, is the glass half empty, half full, or just twice as large as it needs to be?."
//    }
//};