﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageLearningChatBotConsoleClient
{

    namespace WindowsApplication1
    {

        class Program
        {
            private static int controllerID = -1;


            static void Main(string[] args)
            {
                Console.WriteLine("Waiting for server...");
                var getControllerIDTask = getControllerID(LanguageLearningChatBotCore.Language.English, LanguageLearningChatBotCore.Language.French);
                getControllerIDTask.Wait();
                controllerID = getControllerIDTask.Result;

                string userResponse = "1";
                while(true)
                {
                    var respondTask = Respond(userResponse);
                    respondTask.Wait();
                    var serverResponse = respondTask.Result;
                    Console.WriteLine(serverResponse.Prompt.SecondaryText);

                    userResponse = Console.ReadLine();
                }
            }

            private async static Task<int> getControllerID(LanguageLearningChatBotCore.Language primary, LanguageLearningChatBotCore.Language secondary)
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                using (var client = new HttpClient(handler))
                {
                    var values = new Dictionary<string, string>
                    {
                       { "PrimaryLanguage", primary.ToString() },
                       { "SecondaryLanguage", secondary.ToString() }
                    };
                    var content = new FormUrlEncodedContent(values);

                    //var response = await client.GetAsync("https://localhost:5001/api/LanguageLearningChatBotServer");
                    var responseBody = await client.GetStringAsync("https://localhost:5001/api/LanguageLearningChatBotServer");

                    int id = int.Parse(responseBody);

                    Console.OutputEncoding = UnicodeEncoding.UTF8;
                    Console.WriteLine("Instance Id: " + id);
                    return id; 
                }
            }

            private async static Task<LanguageLearningChatBotCore.ResponseAnalysis> Respond(string userResponse)
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                using (var client = new HttpClient(handler))
                {

                    var values = new Dictionary<string, string>
                    {
                       { "InstanceID", controllerID.ToString() },
                       { "Response", userResponse }
                    };
                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("https://localhost:5001/api/LanguageLearningChatBotServer", content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    var responseAnalysis = JsonConvert.DeserializeObject<LanguageLearningChatBotCore.ResponseAnalysis>(responseBody);
                    
                    return responseAnalysis;
                }
            }
        }
    }
}
