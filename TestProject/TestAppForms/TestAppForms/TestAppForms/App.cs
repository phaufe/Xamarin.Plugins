﻿using Connectivity.Plugin;
using DeviceInfo.Plugin;
using Refractored.Xam.TTS;
using Refractored.Xam.TTS.Abstractions;
using Refractored.Xam.Vibrate.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace TestAppForms
{
  public class App
  {
    static ContentPage page;
    static CrossLocale? locale = null;
    public static Page GetMainPage()
    {
      var speakButton = new Button
      {
        Text = "Speak"
      };

      var languageButton = new Button
      {
        Text = "Default Language"
      };

      var sliderPitch = new Slider(0, 2.0, 1.0);
      var sliderRate = new Slider(0, 2.0, Device.OnPlatform(.25, 1.0, 1.0));
      var sliderVolume = new Slider(0, 1.0, 1.0);

      var useDefaults = new Switch
      {
        IsToggled = false
      };

      speakButton.Clicked += (sender, args) =>
        {
          var text = "The quick brown fox jumped over the lazy dog.";
          if (useDefaults.IsToggled)
          {
            CrossTextToSpeech.Current.Speak(text);
            return;
          }

          CrossTextToSpeech.Current.Speak(text,
            pitch: (float)sliderPitch.Value,
            speakRate: (float)sliderRate.Value,
            volume: (float)sliderVolume.Value,
            crossLocale: locale);
        };

      var vibrateButton = new Button
      {
        Text = "Vibrate"
      };

      var sliderVibrate = new Slider(0, 10000.0, 500.0);

      vibrateButton.Clicked += (sender, args) =>
        {
          //var v = DependencyService.Get<IVibrate>();
          //v.Vibration((int)sliderVibrate.Value);
          Refractored.Xam.Vibrate.CrossVibrate.Current.Vibration((int)sliderVibrate.Value);
        };


      var connectivityButton = new Button
      {
        Text = "Connectivity Test"
      };

      var connected = new Label
      {
        Text = "Is Connected: "
      };

      var connectionTypes = new Label
      {
        Text = "Connection Types: "
      };

      var bandwidths = new Label
      {
        Text = "Bandwidths"
      };

      var host = new Entry
      {
        Text = "127.0.0.1"
      };


      var host2 = new Entry
      {
        Text = "montemagno.com"
      };

      var port = new Entry
      {
        Text = "80",
        Keyboard = Keyboard.Numeric
      };

      var canReach1 = new Label
      {
        Text = "Can reach1: "
      };

      var canReach2 = new Label
      {
        Text = "Can reach2: "
      };


      connectivityButton.Clicked += async (sender, args)=>
      {
        connected.Text = CrossConnectivity.Current.IsConnected ? "Connected" : "No Connection";
        bandwidths.Text = "Bandwidths: ";
        foreach(var band in CrossConnectivity.Current.Bandwidths)
        {
          bandwidths.Text += band.ToString() + ", ";
        }
        connectionTypes.Text = "ConnectionTypes:  ";
        foreach(var band in CrossConnectivity.Current.ConnectionTypes)
        {
          connectionTypes.Text += band.ToString() + ", ";
        }

        try
        {
          canReach1.Text = await CrossConnectivity.Current.IsReachable(host.Text) ? "Reachable" : "Not reachable";
        
        }
        catch(Exception ex)
        {

        }
        try
        {
          canReach2.Text = await CrossConnectivity.Current.IsRemoteReachable(host2.Text, int.Parse(port.Text)) ? "Reachable" : "Not reachable";

        }
        catch(Exception ex)
        {

        }
       

      };
      

      languageButton.Clicked += async (sender, args) =>
        {
          var locales = CrossTextToSpeech.Current.GetInstalledLanguages();
          var items = locales.Select(a => a.ToString()).ToArray();

          if (Device.OS == TargetPlatform.Android)
          {
            DependencyService.Get<IDialogs>().DisplayActionSheet("Language", "OK",
                items,
                which =>
                {
                  languageButton.Text = items[which];
                  locale = locales.ElementAt(which);
                }); 
          }
          else
          {
            var selected = await page.DisplayActionSheet("Language", "OK", null, items);
            if (string.IsNullOrWhiteSpace(selected) || selected == "OK")
              return;
            languageButton.Text = selected;
            locale = new CrossLocale { Language = selected };//fine for iOS/WP
          }
        };



      page = new ContentPage
      {
        Content = new ScrollView
        {
          Content = new StackLayout
          {
            Padding = 40,
            Spacing = 10,
            Children = {
              new Label{ Text = "Hello, Forms!"},
              new Label{ Text = "Pitch"},
              sliderPitch,
              new Label{ Text = "Speak Rate"},
              sliderRate,
              new Label{ Text = "Volume"},
              sliderVolume,
              new Label{ Text = "Use Defaults"},
              useDefaults,
              languageButton,
              speakButton,
              new Label{ Text = "Vibrate Length"},
              sliderVibrate,
              vibrateButton,
              new Label{ Text = "Generated AppId: " + CrossDeviceInfo.Current.GenerateAppId()},
              new Label{ Text = "Generated AppId: " + CrossDeviceInfo.Current.GenerateAppId(true)},
              new Label{ Text = "Generated AppId: " + CrossDeviceInfo.Current.GenerateAppId(true, "hello")},
              new Label{ Text = "Generated AppId: " + CrossDeviceInfo.Current.GenerateAppId(true, "hello", "world")},
              new Label{ Text = "Id: " + CrossDeviceInfo.Current.Id},
              new Label{ Text = "Model: " + CrossDeviceInfo.Current.Model},
              new Label{ Text = "Platform: " + CrossDeviceInfo.Current.Platform},
              new Label{ Text = "Version: " + CrossDeviceInfo.Current.Version},
              connectivityButton,
              connected,
              bandwidths,
              connectionTypes,
              host,
              host2,
              port,
              canReach1,
              canReach2,
            }
          }
        }
      };

      return page;
    }
  }
}
