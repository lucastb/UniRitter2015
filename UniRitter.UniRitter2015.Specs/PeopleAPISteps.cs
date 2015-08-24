using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using UniRitter.UniRitter2015.Models;
using UniRitter.UniRitter2015.Services.Implementation;
using UniRitter.UniRitter2015.Support;

namespace UniRitter.UniRitter2015.Specs
{
    [Binding]
    public class PeopleAPISteps
    {
        private readonly HttpClient client;
        private HttpResponseMessage response;
        private string path;

        private Type modelType;
        private Type modelTypeList;
        private IEnumerable<IModel> backgroundData;
        private IModel modelData;
        private IModel resultData;

        public PeopleAPISteps()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:49556/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [When(@"I post it to the /(.+) API endpoint")]
        public void WhenIPostItToTheAPIEndpoint(string path)
        {
            response = client.PostAsJsonAsync(path, modelData).Result;
        }

        private void CheckCode(int code)
        {
            Assert.That(response.StatusCode, Is.EqualTo((HttpStatusCode) code));
        }

        [Then(@"I receive a success \(code (.*)\) return message")]
        public void ThenIReceiveASuccessCodeReturnMessage(int code)
        {
            if (! response.IsSuccessStatusCode)
            {
                var msg = String.Format("API error: {0}", response.Content.ReadAsStringAsync().Result);
                Assert.Fail(msg);
            }
            
            CheckCode(code);
        }
        
        [Then(@"I receive the posted resource")]
        public void ThenIReceiveThePostedResource()
        {
            resultData = (IModel) response.Content.ReadAsAsync(modelType).Result;
            
            switch(modelType.Name)
            {
                case "PersonModel":
                    var personResult = (PersonModel)resultData;
                    var personData = (PersonModel)modelData;
                    Assert.That(personData.firstName, Is.EqualTo(personResult.firstName));
                    break;
                case "PostModel":
                    var postResult = (PostModel)resultData;
                    var postData = (PostModel)modelData;
                    Assert.That(postData.title, Is.EqualTo(postResult.title));
                    break;
                default:
                    ScenarioContext.Current.Pending();
                    break;
            }
        }

        [Then(@"the posted resource now has an ID")]
        public void ThenThePostedResourceNowHasAnID()
        {
            Assert.That(resultData.id, Is.Not.Null);
        }
        
        [Then(@"I receive an error \(code (.*)\) return message")]
        public void ThenIReceiveAnErrorCodeReturnMessage(int code)
        {
            CheckCode(code);
        }

        [Then(@"I receive a message listing all validation errors")]
        public void ThenIReceiveAMessageListingAllValidationErrors()
        {
            var validationMessage = response.Content.ReadAsStringAsync().Result;
            Assert.That(validationMessage, Contains.Substring("firstName"));
            Assert.That(validationMessage, Contains.Substring("email"));
        }

        [Given(@"the populated API")]
        public void GivenThePopulatedAPI()
        {
            // This step has been left blank -- data seeding occurs in the backgorund step
        }

        [When(@"I GET from the /(.+) API endpoint")]
        public void WhenIGETFromTheAPIEndpoint(string path)
        {
            this.path = path;
            response = client.GetAsync(path).Result;
        }

        [Then(@"I get a list containing the populated resources")]
        public void ThenIGetAListContainingThePopulatedResources()
        {
            var resourceList = (IEnumerable<IModel>) response.Content.ReadAsAsync(modelTypeList).Result;
            Assert.That(resourceList, Is.SubsetOf(backgroundData));
        }

        [Then(@"the data matches that id")]
        public void ThenIGetThePersonRecordThatMatchesThatId()
        {
            var id = new Guid(path.Substring(path.LastIndexOf('/') + 1));
            resultData = (IModel) response.Content.ReadAsAsync(modelType).Result;
            var expected = backgroundData.Single(p => p.id == id);
            Assert.That(resultData, Is.EqualTo(expected));
        }

        [Given(@"a person resource as described below:")]
        public void GivenAPersonResourceAsDescribedBelow(Table table)
        {
            modelData = new PersonModel();
            table.FillInstance((PersonModel)modelData);
        }

        [Then(@"I can fetch it from the /(.+) API endpoint")]
        public void ThenICanFetchItFromTheAPIEndpoint(string path)
        {
            var id = resultData.id.Value;
            var newEntry = client.GetAsync(path + "/" + id).Result;
            Assert.That(newEntry, Is.Not.Null);
        }
        
        [Given(@"a ""(.*)"" resource")]
        public void GivenAResource(string p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"(.+) resource")]
        public void GivenAnInvalidResource(string resourceCase)
        {
            // step purposefully left blank
        }

        [Given(@"an API populated with the following people")]
        public void GivenAnAPIPopulatedWithTheFollowingPeople(Table table)
        {
            modelType = typeof(PersonModel);
            modelTypeList = typeof(IEnumerable<PersonModel>);
            backgroundData = table.CreateSet<PersonModel>();
            
            var repo = new InMemoryRepository<PersonModel>();
            foreach (var entry in backgroundData) {
                repo.Add((PersonModel) entry);
            }
        }

        [When(@"I post the following data to the /(.+) API endpoint: (.+)")]
        public void WhenIPostTheFollowingDataToThePeopleAPIEndpoint(string path, string jsonData)
        {
            switch(modelType.Name)
            {
                case "PersonModel":
                    modelData = JsonConvert.DeserializeObject<PersonModel>(jsonData);
                    response = client.PostAsJsonAsync(path, modelData).Result;
                    break;
                case "PostModel":
                    modelData = JsonConvert.DeserializeObject<PostModel>(jsonData);
                    response = client.PostAsJsonAsync(path, modelData).Result;
                    break;
                default:
                    ScenarioContext.Current.Pending();
                    break;
            }
            
        }
        
        [Then(@"I receive a message that conforms (.+)")]
        public void ThenIReceiveAMessageThatConforms(string pattern)
        {
            var msg = response.Content.ReadAsStringAsync().Result;
            StringAssert.IsMatch(pattern, msg);
        }
        
        [Given(@"an API populated with the following posts")]
        public void GivenAnAPIPopulatedWithTheFollowingPosts(Table table)
        {
            modelType = typeof(PostModel);
            modelTypeList = typeof(IEnumerable<PostModel>);
            backgroundData = table.CreateSet<PostModel>();
            
            var repo = new InMemoryRepository<PostModel>();
            foreach (var entry in backgroundData)
            {
                repo.Add((PostModel) entry);
            }
        }

        [Given(@"a post resource as described below:")]
        public void GivenAPostResourceAsDescribedBelow(Table table)
        {
            modelData = new PostModel();
            table.FillInstance((PostModel) modelData);
        }
    }
}