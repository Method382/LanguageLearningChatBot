using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
// NOTE: Install the Newtonsoft.Json NuGet package.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LanguageLearningChatBotCore.DataModels;
using LanguageLearningChatBotCore.Clients;
using LanguageLearningChatBotCore.DataModels.BingSpellCheck;
using Microsoft.EntityFrameworkCore;

namespace LanguageLearningChatBotCore
{
    public class ControllerAPIImpl
    {
        static string host = "https://api.cognitive.microsofttranslator.com";
        static string path = "/translate?api-version=3.0";
        static string params_ = "&to=";

        // Text Translator API key
        static string key = "fae421f860b548ecb7c5c5b04f12826a";

        private int responseCount = 0;
        public async static Task<TranslationData> Translate(string data, Language toLang)
        {
            System.Object[] body = new System.Object[] { new { Text = data } };
            var requestBody = JsonConvert.SerializeObject(body);

            string uri = host + path + params_ + LanguageLookup.Languages[(int)toLang];

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                // remove [] surrounding json response
                TranslatorResponse result = JsonConvert.DeserializeObject<TranslatorResponse>(responseBody.Substring(1, responseBody.Length-2));

                TranslationData translationData = new TranslationData();
                translationData.SecondaryLanguage = LanguageLookup.FindLanguage(result.Translations[0].To);
                translationData.SecondaryText = result.Translations[0].Text;
                translationData.PrimaryLanguage = LanguageLookup.FindLanguage(result.DetectedLanguage.Language);
                translationData.PrimaryText = data;
                return translationData;
            }
        }

        public async Task<List<Correction>> SpellCheck(string data, Language toLang)
        {
            var spellCheckClient = new SpellCheckClient(toLang);
            SpellCheckResponse response = await spellCheckClient.SpellCheck(data);
            var corrections = new List<Correction>();
            foreach (FlaggedToken flaggedToken in response.FlaggedTokens)
            {
                var correction = new Correction(flaggedToken);
                corrections.Add(correction);
            }
            return corrections;
        }

        public ResponseAnalysis Respond(Language primary, Language secondary, string response)
        {

            TranslationData myPrompt = null;
            using (var db = new LanguageLearningChatBotCore.ScenarioContext())
            {
                var myPrompts = db.Prompts.Where(prompt => /*prompt.ScenarioID == myScenario.ScenarioID &&*/ prompt.ResponseNumber == responseCount).ToList();
                if(myPrompts.Count > 0)
                {
                    myPrompt = Translate(myPrompts[0].PromptText, secondary).Result;
                }
                else
                {
                    myPrompt = Translate("Scenario Over", secondary).Result;
                }
            }

            responseCount++;

            var theirResponse = Translate(response, secondary).Result;

            var responseAnalysis = new ResponseAnalysis();
            responseAnalysis.Response = theirResponse;
            responseAnalysis.Prompt = myPrompt;
            responseAnalysis.Corrections = SpellCheck(response, secondary).Result;
            return responseAnalysis;
        }
    }
}