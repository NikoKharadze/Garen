using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using Newtonsoft.Json;

namespace App1
{
    [Activity(Label = "QuizActivity")]
    public class QuizActivity : Activity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private const string BaseUrl = "http://10.42.134.5:8000";
        private const string Endpoint = "/api/questions";
        private TextView questions;
        private RadioButton opt1;
        private RadioButton opt2;
        private RadioButton opt3;
        private Button sub;
        private List<Question> quizQuestions;
        private int currentQuestionIndex;
        bool point = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Quiz);

            questions = FindViewById<TextView>(Resource.Id.question);
            opt1 = FindViewById<RadioButton>(Resource.Id.ans1);
            opt2 = FindViewById<RadioButton>(Resource.Id.ans2);
            opt3 = FindViewById<RadioButton>(Resource.Id.ans3);
            sub = FindViewById<Button>(Resource.Id.btn_sub);
            sub.Click += CheckAnswer;
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation6);
            navigation.SetOnNavigationItemSelectedListener(this);
            // Start the task to fetch JSON data
            FetchDataAsync();
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    StartActivity(typeof(MainActivity));
                    return true;
                case Resource.Id.navigation_offer:
                    // Open a new activity for Offers
                    StartActivity(typeof(offersActivity));
                    return true;
                case Resource.Id.navigation_notifications:
                    StartActivity(typeof(shopActivity));
                    return true;
            }
            return false;
        }

        private async void FetchDataAsync()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(BaseUrl);

                    HttpResponseMessage response = await httpClient.GetAsync(Endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        QuizData quizData = JsonConvert.DeserializeObject<QuizData>(jsonContent);

                        // Ensure there are enough questions
                        if (quizData.Questions.Count >= 3)
                        {
                            quizQuestions = quizData.Questions;

                            // Shuffle the questions to get random ones
                            ShuffleQuestions();

                            // Load the first question
                            LoadQuestion();
                        }
                        else
                        {
                            Console.WriteLine("There are not enough questions available.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        private void ShuffleQuestions()
        {
            Random rng = new Random();
            int n = quizQuestions.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Question value = quizQuestions[k];
                quizQuestions[k] = quizQuestions[n];
                quizQuestions[n] = value;
            }
        }

        private void LoadQuestion()
        {
            // Check if there are remaining questions
            if (currentQuestionIndex < quizQuestions.Count)
            {
                Question currentQuestion = quizQuestions[currentQuestionIndex];

                string questionText = currentQuestion.QuestionText;
                questions.Text = questionText;
                List<string> options = currentQuestion.Options;
                opt1.Text = options[0];
                opt2.Text = options[1];
                opt3.Text = options[2];

                currentQuestionIndex++;

                // Check if it's the third question, then navigate to OffersActivity
                if (currentQuestionIndex > 3)
                {
                    NavigateToOffersActivity();
                }
            }
            else
            {
                // No more questions, show a message or navigate to another activity
                Toast.MakeText(this, "Quiz completed!", ToastLength.Short).Show();
                NavigateToOffersActivity();
            }
        }

        private void CheckAnswer(object sender, EventArgs args)
        {
            // Find the selected answer
            string selectedAnswer = null;
            if (opt1.Checked)
                selectedAnswer = "a";
            else if (opt2.Checked)
                selectedAnswer = "b";
            else if (opt3.Checked)
                selectedAnswer = "c";

            // Compare the selected answer with the correct answer
            if (selectedAnswer == quizQuestions[currentQuestionIndex - 1].Answer)
            {
                Toast.MakeText(this, "Correct!", ToastLength.Short).Show();
            }
            else
            {
                point = false;
                Toast.MakeText(this, $"Incorrect. The correct answer is {quizQuestions[currentQuestionIndex - 1].Answer}.", ToastLength.Short).Show();
            }

            // Load the next question
            LoadQuestion();
        }

        private void NavigateToOffersActivity()
        {
            Intent intent = new Intent(this, typeof(offersActivity));
            StartActivity(intent);
            if (point)
            {
                SetResult(Result.Ok, intent);
            }
            else
            {
                SetResult(Result.Canceled, intent);
            }
            Finish();
        }

        // Define the QuizData and Question classes
        public class QuizData
        {
            [JsonProperty("questions")]
            public List<Question> Questions { get; set; }
        }

        public class Question
        {
            [JsonProperty("question")]
            public string QuestionText { get; set; }

            [JsonProperty("options")]
            public List<string> Options { get; set; }

            [JsonProperty("answer")]
            public string Answer { get; set; }
        }
    }
}
