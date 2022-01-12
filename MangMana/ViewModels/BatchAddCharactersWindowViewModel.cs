using MangMana.Helpers;
using MangMana.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace MangMana.ViewModels
{
    internal class BatchAddCharactersWindowViewModel : BaseViewModel
    {
        public string Text { get; set; }

        public ICommand ShowInfoMessageCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand CloseCommand { get; }

        public Action CloseWindow;

        private List<Character> _charactersList;

        public BatchAddCharactersWindowViewModel(List<Character> charactersList)
        {
            _charactersList = charactersList;

            ShowInfoMessageCommand = new SimpleCommand(ShowInfoMessage);
            ImportCommand = new SimpleCommand(Import);
            CloseCommand = new SimpleCommand(() => CloseWindow.Invoke());
        }

        private void ShowInfoMessage()
        {
            string message = "The text gets interpreted by the following format:\n\nCharacter Name - Character Description\n\nEmpty lines will become separators (--------), and any other text will require user's decision.";
            MessageBox.Show(message, "How to");
        }

        private void Import()
        {
            //process it
            if (string.IsNullOrEmpty(Text))
            {
                MessageBox.Show("Input field is empty.", "Cannot import nothing");
                return;
            }

            string[] lines = Text.Split("\r\n");

            foreach (var line in lines)
            {
                Character character = new Character();

                if (!string.IsNullOrEmpty(line))
                {
                    string[] results = line.Split(" - ");

                    if (results.Length < 2)
                    {
                        var messageResult = MessageBox.Show($"Problem while importing, couldn't interpret following line:\n\n{line}\n\nDo you want to add it as simple entry?", "Problem with import", MessageBoxButton.YesNoCancel);
                        if (messageResult == MessageBoxResult.Cancel)
                            return;
                        if (messageResult == MessageBoxResult.No)
                            continue;

                        character.Name = line;
                    }
                    else
                    {
                        character.Name = results[0].Trim();

                        for (int i = 1; i < results.Length; i++)
                        {
                            if (i > 1) character.Description += " - ";
                            character.Description += results[i];
                        }
                    }
                }

                _charactersList.Add(character);
            }

            CloseWindow.Invoke();
        }
    }
}