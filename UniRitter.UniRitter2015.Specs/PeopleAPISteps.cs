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
        private Type modelType;
        private IEnumerable<IModel> backgroundData;
        private string path;
        private IModel modelData;
        private HttpResponseMessage response;
        private IModel resultData;

        public PeopleAPISteps()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:49556/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [When(@"I post it to the /people API endpoint")]
        public void WhenIPostItToThePeopleAPIEndpoint()
        {
            response = client.PostAsJsonAsync("people", modelData).Result;
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

        private void checkPersonData(Person p)
        {

        }

        [Then(@"I receive the posted resource")]
        public void ThenIReceiveThePostedResource()
        {
            resultData = (IModel)response.Content.ReadAsAsync(modelType).Result;
            //Assert.That(resultData.firstName, Is.EqualTo(modelData.firstName));
            // switch
        }

        [Then(@"I receive the posted resource for post")]
        public void ThenIReceiveThePostedResourceForPost()
        {
            resultData = response.Content.ReadAsAsync<PostModel>().Result;
            Assert.That(resultData.title, Is.EqualTo(modelData.title));
        }

        [Then(@"the posted resource now has an ID")]
        public void ThenThePostedResourceNowHasAnID()
        {
            Assert.That(resultData.id, Is.Not.Null);
        }

        [Then(@"the posted resource now has an ID for post")]
        public void ThenThePostedResourceNowHasAnIDForPost()
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
            var resourceList = response.Content.ReadAsAsync<IEnumerable<Person>>().Result;
            Assert.That(backgroundData, Is.SubsetOf(resourceList));
        }

        [Then(@"I get a list containing the populated resources from post")]
        public void ThenIGetAListContainingThePopulatedResourcesFromPost()
        {
            var resourceList = response.Content.ReadAsAsync<IEnumerable<PostModel>>().Result;
            Assert.That(backgroundData, Is.SubsetOf(resourceList));
        }

        [Then(@"the data matches that id")]
        public void ThenIGetThePersonRecordThatMatchesThatId()
        {
            var id = new Guid(path.Substring(path.LastIndexOf('/') + 1));
            resultData = response.Content.ReadAsAsync<Person>().Result;
            var expected = backgroundData.Single(p => p.id == id);
            Assert.That(resultData, Is.EqualTo(expected));
        }

        [Given(@"a person resource as described below:")]
        public void GivenAPersonResourceAsDescribedBelow(Table table)
        {
            modelData = new Person();
            table.FillInstance(modelData);
        }

        [Then(@"I can fetch it from the API")]
        public void ThenICanFetchItFromTheAPI()
        {
            var id = resultData.id.Value;
            var newEntry = client.GetAsync("people/" + id).Result;
            Assert.That(newEntry, Is.Not.Null);
        }

        [Then(@"I can fetch it from the API for post")]
        public void ThenICanFetchItFromTheAPIForPost()
        {
            var id = resultData.id.Value;
            var newEntry = client.GetAsync("posts/" + id).Result;
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
            backgroundData = table.CreateSet<PersonModel>();

            //var mongoRepo = new MongoRepository<PersonModel>(new ApiConfig());
            //mongoRepo.Upsert(table.CreateSet<PersonModel>());

            var repo = new InMemoryRepository<PersonModel>();
            foreach (var entry in table.CreateSet<PersonModel>()) {
                repo.Add(entry);
            }
        }

        [When(@"I post the following data to the /people API endpoint: (.+)")]
        public void WhenIPostTheFollowingDataToThePeopleAPIEndpoint(string jsonData)
        {
            modelData = JsonConvert.DeserializeObject<modelType>(jsonData);
            response = client.PostAsJsonAsync("people", modelData).Result;
        }

        [Then(@"I receive a message that conforms (.+)")]
        public void ThenIReceiveAMessageThatConforms(string pattern)
        {
            var msg = response.Content.ReadAsStringAsync().Result;
            StringAssert.IsMatch(pattern, msg);
        }

        private class Person : IModel
        {
            public Guid? id { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }
            public string url { get; set; }

            public bool Equals(Person other)
            {
                if (other == null) return false;

                return
                    id == other.id
                    && firstName == other.firstName
                    && lastName == other.lastName
                    && email == other.email
                    && url == other.url;
            }

            public override bool Equals(object obj)
            {
                if (obj != null)
                {
                    return Equals(obj as Person);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return id.GetHashCode();
            }
        }

        // private IEnumerable<PostModel> backgroundPostData;
        // private PostModel postData;
        // private PostModel resultPost;

        [Given(@"an API populated with the following posts")]
        public void GivenAnAPIPopulatedWithTheFollowingPosts(Table table)
        {
            modelType = typeof(PostModel);
            backgroundData = table.CreateSet<PostModel>();
            
            var repo = new InMemoryRepository<PostModel>();
            foreach (var entry in table.CreateSet<PostModel>())
            {
                repo.Add(entry);
            }
        }

        [Given(@"a post resource as described below:")]
        public void GivenAPostResourceAsDescribedBelow(Table table)
        {
            modelData = new PostModel();
            table.FillInstance(modelData);
        }

        [When(@"I post it to the /posts API endpoint")]
        public void WhenIPostItToThePostsAPIEndpoint()
        {
            response = client.PostAsJsonAsync("posts", modelData).Result;
        }

        [When(@"I post the following data to the /posts API endpoint: \{}")]
        public void WhenIPostTheFollowingDataToThePostsAPIEndpoint(string jsonData)
        {
            modelData = JsonConvert.DeserializeObject<PostModel>(jsonData);
            response = client.PostAsJsonAsync("posts", modelData).Result;
        }

    }
}