using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Data.Sqlite;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Android.Webkit;

namespace ViktorApp
{
    [Activity(Label = "Activity1")]
    public class Quizz : Activity
    {
        string connectionString;
        string pathToDatabase;
        string appFolder = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
        int qNumber;
        //List<string> qList = new List<string>();
        List<string> answers = new List<string>();
        List<int> correctAnswers = new List<int>();
        string currentAnswer;
        string correctAnswerId;
        string correctAnswer;
        string question = "";
        string answer1 = "";
        string answer2 = "";
        string answer3 = "";
        string answer4 = "";

        TextView txtNumber;
        TextView txtQuestion;
        Button btnAns1;
        Button btnAns2;
        Button btnAns3;
        Button btnAns4;
        Button buttonCheat;
        Spinner spinAnswer;
        Button buttonConfirm;


        List<KeyValuePair<int, string>> questionList = new List<KeyValuePair<int, string>>();
        string catName;
        int catIndex;
        string score;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            catIndex = this.Intent.GetIntExtra("catIndex", 0);
            catName = this.Intent.GetStringExtra("catName");
            SetContentView(Resource.Layout.quizz);


            txtNumber = FindViewById<TextView>(Resource.Id.txtNumber);
            txtQuestion = FindViewById<TextView>(Resource.Id.txtQuestion);
            //btnAns1 = FindViewById<Button>(Resource.Id.btnAns1);
            //btnAns2 = FindViewById<Button>(Resource.Id.btnAns2);
            //btnAns3 = FindViewById<Button>(Resource.Id.btnAns3);
            //btnAns4 = FindViewById<Button>(Resource.Id.btnAns4);
            //buttonCheat = FindViewById<Button>(Resource.Id.btnCheat);

            buttonCheat.Click += (sender, args) =>
            {
                if (sender is Button btnCheat)
                {
                    Toast.MakeText(this, "Cheater!", ToastLength.Short).Show();
                    try
                    {
                        Intent intent = new Intent(this, typeof(Result));
                        StartActivity(intent);
                        Toast.MakeText(this, "Cheater!", ToastLength.Short).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    }
                }
            };

            //Toast.MakeText(this, catName, ToastLength.Short).Show();
            pathToDatabase = Path.Combine(appFolder, "databaseVApp.db");
            connectionString = $"Data Source={pathToDatabase};";

            CreateDB();

            if (System.IO.File.Exists(pathToDatabase))
            {
                //Toast.MakeText(this, "Database allready exists", ToastLength.Short).Show();
                GetQuestions("select * from quizz where category=@category", "category", catName);
                GetCorrectAnswer("select correctAnswer from quizz where id=@id", "id", correctAnswerId);
                InsertResult("insert into results (result) values (@result)", "result", score);
                return; 
            } else
            using (var source = Resources.OpenRawResource(Resource.Raw.databaseVApp))
            {
                using (var destination = System.IO.File.Create(pathToDatabase))
                {
                    source.CopyTo(destination);
                }
            }
        }

        // not finished
        public async void GetQuestions(string SQLgetQuestionSet, string paramName, string paramValue)
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
                            //int n = 1;
                            reader.Read();
                            while (reader.Read()) 
                            {
                                qNumber++;
                                this.answers.Clear();
                                this.correctAnswerId = reader.GetString(0);
                                this.question = reader.GetString(1);
                                this.answer1 = reader.GetString(2);
                                this.correctAnswer = reader.GetString(2);
                                this.answer2 = reader.GetString(3);
                                this.answer3 = reader.GetString(4);
                                this.answer4 = reader.GetString(5);
                                SetContent();
                                var adapter = new ArrayAdapter<string>
               (this, Android.Resource.Layout.SimpleSpinnerDropDownItem, answers);

                                spinAnswer.Adapter = adapter;

                                buttonConfirm.Click += (sender, args) =>
                                {
                                    if (spinAnswer.SelectedItem != null)
                                    {
                                        this.currentAnswer = spinAnswer.SelectedItem.ToString();
                                        CheckAnswer();
                                    }
                                };
                            }
                        }
                    }
                    dbConn.Close();
                }
            } catch (Exception ex) {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }



        public async void GetCorrectAnswer(string SQLgetQuestionSet, string paramName, string paramValue)
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
                            this.correctAnswer = reader.ToString();
                        }
                        Toast.MakeText(this, correctAnswerId, ToastLength.Long).Show();

                    }
                    dbConn.Close();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }




        public void SetContent()
        {
           

            //question = qList[1].ToString();
            //this.answer1 = qList[2].ToString();
            //this.answer2 = qList[3].ToString();
            //this.answer3 = qList[4].ToString();
            //this.answer4 = qList[5].ToString();

            this.answers.Add(answer1);
            this.answers.Add(answer2);
            this.answers.Add(answer3);
            this.answers.Add(answer4);

            spinAnswer = FindViewById<Spinner>(Resource.Id.answer);
            buttonConfirm = FindViewById<Button>(Resource.Id.btnConfirm);




            txtNumber.Text = "Question number: " + qNumber.ToString();
            txtQuestion.Text = question;

            //btnAns1.Text = answers[0].ToString();
            //btnAns2.Text = answers[1].ToString();
            //btnAns3.Text = answers[2].ToString();
            //btnAns4.Text = answers[3].ToString();
        }

        public void CheckAnswer() //not finished
        {
            if (currentAnswer == correctAnswer)
            {
                correctAnswers.Add(1);
            }else
            {
                correctAnswers.Add(0);
            }
            //if (correctAnswers.Count = 10) 
            //{
            //    InsertResult("score", score);
            //}
        }

        public async void InsertResult(string SQLInsertResult, string paramName, string paramValue)
        {
            try
            {
                using (var dbConn = new SqliteConnection(connectionString))
                {
                    dbConn.Open();
                    using (SqliteCommand cmd = new SqliteCommand(SQLInsertResult, dbConn))
                    {
                        cmd.Parameters.AddWithValue(paramName, paramValue);
                        cmd.ExecuteNonQuery();
                    }
                    dbConn.Close();
                }
            } catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
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

        //private static Random rng = new Random();

        //public static List<T> Shuffle<T>(this List<T> list)
        //{
        //    int n = list.Count;
        //    while (n > 1)
        //    {
        //        n--;
        //        int k = rng.Next(n + 1);
        //        T value = list[k];
        //        list[k] = list[n];
        //        list[n] = value;
        //    }
        //    return list;
        //}
    }

}