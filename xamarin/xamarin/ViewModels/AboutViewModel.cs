using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Input;
using xamarin.Models;
using xamarin.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using PassbookTicketGenerator;
using ElectronicTicketGenerator;

namespace xamarin.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        //IElectronicTicketGenerator passBookGenerator;

        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            BindingDeviceCommand = new Command(OnBindingDevice);
        }

        public ICommand OpenWebCommand { get; }
        public Command BindingDeviceCommand { get; }

#pragma warning disable CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica
        private async void OnBindingDevice() {
            Console.WriteLine("Device vinculado");
            //_ = Shell.Current.GoToAsync("..");
            //object value = await binding();// Console.WriteLine("Device vinculado");
            
            List<TicketData> tickets = new List<TicketData>();

            //if (passBookGenerator is not PassbookGenerator pkpassGenerator) Console.WriteLine("Error...");
            PassbookGenerator pkpassGenerator = new PassbookGenerator();
            var bindingUri = await pkpassGenerator.GenerateBindingRequest(tickets.FirstOrDefault());

        }
    }
}
