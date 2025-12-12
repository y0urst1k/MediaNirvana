using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MediaTracker.Views
{
    public partial class ListEditorDialog : Window
    {
        public PersonalList ResultList { get; private set; }
        private string _originalId;

        public ListEditorDialog(PersonalList listToEdit = null)
        {
            InitializeComponent();

            if (listToEdit != null)
            {
                // Режим редактирования
                Title = "Edit List";
                _originalId = listToEdit.Id;
                NameInput.Text = listToEdit.Name;
                DescInput.Text = listToEdit.Description;

                foreach (ComboBoxItem item in TypeInput.Items)
                    if (item.Content.ToString() == listToEdit.Type) TypeInput.SelectedItem = item;

                // Копируем существующие ID, чтобы не потерять их
                ResultList = new PersonalList { ItemIds = new List<string>(listToEdit.ItemIds) };
            }
            else
            {
                // Режим создания
                Title = "New List";
                ResultList = new PersonalList { ItemIds = new List<string>() };
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text)) return;

            ResultList.Id = _originalId ?? DateTime.Now.Ticks.ToString();
            ResultList.Name = NameInput.Text;
            ResultList.Description = DescInput.Text;
            ResultList.Type = (TypeInput.SelectedItem as ComboBoxItem).Content.ToString();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}