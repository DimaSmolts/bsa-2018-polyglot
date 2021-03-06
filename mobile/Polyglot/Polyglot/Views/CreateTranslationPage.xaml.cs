﻿using Polyglot.BusinessLogic;
using Polyglot.BusinessLogic.DTO;
using Polyglot.BusinessLogic.Services;
using Polyglot.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Polyglot.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateTranslationPage : ContentPage
    {
        public int ComplexStringId { get; set; }
        public TranslationViewModel Translation { get; set; }
        public UserDTO User { get; set; }

        public CreateTranslationPage(int complexStringId, TranslationViewModel translation)
        {
            BindingContext = translation;

            ComplexStringId = complexStringId;
            Translation = translation;
            User = UserService.CurrentUser;

            InitializeComponent();
        }

        private async void SaveTranslation_Clicked(object sender, EventArgs e)
        {
            var httpService = new HttpService();
            var translationsUrl = "complexstrings/" + ComplexStringId + "/translations";


            var editedTranslation = new TranslationDTO
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                LanguageId = Translation.LanguageId,
                TranslationValue = Translation.Translation,
                UserId = User.Id
            };

            if (string.IsNullOrEmpty(editedTranslation.TranslationValue)|| editedTranslation.TranslationValue=="Not translated")
            {
                return;
            }

            var translationResult = await httpService.PostAsync<TranslationDTO>(translationsUrl, editedTranslation);

            if (translationResult != null)
            {
                Translation.Id = translationResult.Id.ToString();
                await DisplayAlert("Result", "Translation saved!", "Ok");
                await Navigation.PopAsync();
            }
        }
    }
}