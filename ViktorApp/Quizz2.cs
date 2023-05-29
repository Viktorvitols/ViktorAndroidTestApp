using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ViktorApp
{
    [Activity(Label = "Quizz2")]
    public class Quizz2 : Activity
    {
        string catName;
        string connectionString;
        string pathToDatabase;
        string appFolder = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;

        string question;
        int qNumber;
        List<string> answers = new List<string>();
        string correctAnswer;
        string currentAnswer;

        TextView txtNumber;
        TextView txtQuestion;
        Spinner spinAnswer;
        Button buttonConfirm;

        string SQLGetQuestions = "select * from quizz where category=@category";

        List<int> scoreList = new List<int>();
        string score;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            catName = this.Intent.GetStringExtra("catName");
            SetContentView(Resource.Layout.quizz);

            txtNumber = FindViewById<TextView>(Resource.Id.txtNumber);
            txtQuestion = FindViewById<TextView>(Resource.Id.txtQuestion);

            pathToDatabase = Path.Combine(appFolder, "databaseVApp.db");
            connectionString = $"Data Source={pathToDatabase};";

            CreateDB();

            if (System.IO.File.Exists(pathToDatabase))
            {
                GetQuestions(SQLGetQuestions, "category", catName, qNumber);
                
                try
                {
                    buttonConfirm.Click += (sender, args) =>
                {
                    this.currentAnswer = spinAnswer.SelectedItem.ToString();
                    CheckAnswer(currentAnswer, correctAnswer);
                    SetContent(qNumber);

                };
                  
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            
                




                return;
            }
            else
                using (var source = Resources.OpenRawResource(Resource.Raw.databaseVApp))
                {
                    using (var destination = System.IO.File.Create(pathToDatabase))
                    {
                        source.CopyTo(destination);
                    }
                }

        }



        private void CreateDB()
        {
            string SQLDB = "CREATE TABLE IF NOT EXISTS quizz (id INTEGER PRIMARY KEY AUTOINCREMENT, question TEXT UNIQUE, correctAnswer TEXT, wrongAnswer1 TEXT, wrongAnswer2 TEXT, wrongAnswer3 TEXT, category TEXT);" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (22, 'x + 10 = 18', '8', '10', '30', '1', 'Addition');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (23, 'x + 35 = 40', '5', '3', '10', '8', 'Addition');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (24, 'x + 100 = 310', '210', '300', '10', '211', 'Addition');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (25, 'x + 7 = 10', '17', '10', '1', '20', 'Addition');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (26, 'x + 1000 = 1111', '50', '111', '911', '1', 'Addition');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (27, 'x + 1 = 101', '150', '3', '100', '50', 'Addition');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (28, 'x + 30 = 40', '10', '50', '53', '18', 'Addition');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (29, '55 + x = 60', '5', '15', '1', '18', 'Addition');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (30, '1111 + x = 11111', '10000', '1000', '11111', '1', 'Addition');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (31, '105 + x = 110', '5', '38', '56', '120', 'Addition');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (32, 'x - 100 = 10', '110', '90', '50', '10', 'Subtraction');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (33, 'x - 7 = 10', '17', '3', '10', '20', 'Subtraction');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (34, 'x - 200 = 300', '500', '100', '600', '200', 'Subtraction');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (35, '50 - x = 25', '25', '50', '10', '15', 'Subtraction');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (36, '2222 - x = 1111', '1111', '111', '1000', '2', 'Subtraction');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (37, 'x - 2000 = 1000', '3000', '1010', '1001', '2000', 'Subtraction');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (38, 'x - 58 = 8', '64', '68', '58', '50', 'Subtraction');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (39, 'x - 0 = 123', '123', '132', '321', '213', 'Subtraction');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (40, ' 987 - x = 500', '487', '478', '1000', '3', 'Subtraction');" +
                "INSERT INTO quizz (id, question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (41, '258 - x = 13', '245', '351', '145', '32', 'Subtraction');";

            if (File.Exists(pathToDatabase) == false)
            {
                try
                {
                    using (var dbConn = new SqliteConnection(connectionString))
                    {
                        dbConn.Open();
                        using (SqliteCommand cmd = new SqliteCommand(SQLDB, dbConn))
                        {
                            int response = cmd.ExecuteNonQuery();
                            Toast.MakeText(this, $"{response} rows affected", ToastLength.Short).Show();
                        }
                        dbConn.Close();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "Db allready exists", ToastLength.Short).Show();
            }
        }

        public async void GetQuestions(string SQLgetQuestionSet, string paramName, string paramValue, int qNumber)
        {
            try
            {
                using (var dbConn = new SqliteConnection(connectionString))
                {
                    dbConn.Open();
                    using (SqliteCommand cmd = new SqliteCommand(SQLgetQuestionSet, dbConn))
                    {
                        cmd.Parameters.AddWithValue(paramName, paramValue);
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            for (int i = qNumber; qNumber < reader.FieldCount; qNumber++) 
                            {
                                this.answers.Clear();
                                this.question = reader.GetString(1);
                                this.correctAnswer = reader.GetString(2);
                                this.answers.Add(reader.GetString(2));
                                this.answers.Add(reader.GetString(3));
                                this.answers.Add(reader.GetString(4));
                                this.answers.Add(reader.GetString(5));
                                SetContent(qNumber);
                                break;

                            }
                            if (qNumber == reader.FieldCount)
                            {
                                try
                                {
                                    Intent intent = new Intent(this, typeof(Result));
                                    intent.PutExtra("Score", score);
                                    StartActivity(intent);
                                    
                                }
                                catch (Exception ex)
                                {
                                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                                }
                            }
                        }
                    } dbConn.Close();
                }
            } catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        public void SetContent(int qNumber)
        {
            qNumber++;
            txtNumber.Text = "Question number: " + qNumber.ToString();
            txtQuestion.Text = question;
            buttonConfirm = FindViewById<Button>(Resource.Id.btnConfirm);
            spinAnswer = FindViewById<Spinner>(Resource.Id.answer);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, answers);
            spinAnswer.Adapter = adapter;
        }

        public void CheckAnswer(string currentAnswer, string correctAnswer)
        {
            if (currentAnswer == correctAnswer)
            {
                this.scoreList.Add(1);
                Toast.MakeText(this, "Correct", ToastLength.Short).Show();
            }
            else
            {
                this.scoreList.Add(0);
                Toast.MakeText(this, "Wrong", ToastLength.Short).Show();
            }

        }

        public string GetScore(List<int> scoreList)
        {
            int maxScore = scoreList.Count;
            int yourScore = scoreList.Sum();
            string score = yourScore.ToString() + " / " + maxScore.ToString();
            return score;
        }

    }
}