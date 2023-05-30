using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Android.Provider.Telephony.Mms;

namespace ViktorApp
{
    [Activity(Label = "Quizz2")]
    public class Quizz2 : Activity
    {
        string catName;
        string connectionString;
        string pathToDatabase;
        string appFolder = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;

        TextView txtNumber;
        TextView txtQuestion;
        Spinner spinAnswer;
        Button buttonConfirm;

        string SQLQuTableCreate = "CREATE TABLE IF NOT EXISTS quizz (id INTEGER PRIMARY KEY AUTOINCREMENT, question TEXT UNIQUE, correctAnswer TEXT, wrongAnswer1 TEXT, wrongAnswer2 TEXT, wrongAnswer3 TEXT, category TEXT);" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x + 10 = 18', '8', '10', '30', '1', 'Addition');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x + 35 = 40', '5', '3', '10', '8', 'Addition');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x + 100 = 310', '210', '300', '10', '211', 'Addition');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x + 7 = 10', '17', '10', '1', '20', 'Addition');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x + 1000 = 1111', '50', '111', '911', '1', 'Addition');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x + 1 = 101', '150', '3', '100', '50', 'Addition');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x + 30 = 40', '10', '50', '53', '18', 'Addition');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('55 + x = 60', '5', '15', '1', '18', 'Addition');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('1111 + x = 11111', '10000', '1000', '11111', '1', 'Addition');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('105 + x = 110', '5', '38', '56', '120', 'Addition');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x - 100 = 10', '110', '90', '50', '10', 'Subtraction');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x - 7 = 10', '17', '3', '10', '20', 'Subtraction');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x - 200 = 300', '500', '100', '600', '200', 'Subtraction');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('50 - x = 25', '25', '50', '10', '15', 'Subtraction');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('2222 - x = 1111', '1111', '111', '1000', '2', 'Subtraction');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x - 2000 = 1000', '3000', '1010', '1001', '2000', 'Subtraction');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x - 58 = 8', '64', '68', '58', '50', 'Subtraction');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('x - 0 = 123', '123', '132', '321', '213', 'Subtraction');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES (' 987 - x = 500', '487', '478', '1000', '3', 'Subtraction');" +
                "INSERT INTO quizz (question, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3, category) VALUES ('258 - x = 13', '245', '351', '145', '32', 'Subtraction');";
        string SQLResTableCreate = "CREATE TABLE results (id INTEGER  PRIMARY KEY AUTOINCREMENT, result TEXT, Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)";
        string SQLGetQuestions = "select * from quizz where category=@category";
        string SQLInsertResult = "insert into results (result) values (@result)";
        int qNumber;
        string currentAnswer;

        QuAns quAns = new QuAns();
        List<QuAns> quAnsList = new List<QuAns>();
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
            CreateDB();
            qNumber = 0;

            if (System.IO.File.Exists(pathToDatabase))
            {
                quAnsList = GetQuestions(SQLGetQuestions, "category", catName);
                SetContent(quAnsList, qNumber);
                ConfirmAnswer();
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


        private void ConfirmAnswer()
        {
            try
            {
                buttonConfirm.Click += (sender, args) =>
                {
                    this.currentAnswer = spinAnswer.SelectedItem.ToString();
                    CheckAnswer(currentAnswer, quAns.correctAnswer);
                    qNumber++;
                    if (qNumber < quAnsList.Count)
                    {
                        SetContent(quAnsList, qNumber);
                    }
                    else
                    {
                        FinishQuizz(scoreList);
                    }
                };

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            return;
        }

        private void CreateDB()
        {
            if (File.Exists(pathToDatabase) == false)
            {
                try
                {
                    using (var dbConn = new SqliteConnection(connectionString))
                    {
                        dbConn.Open();
                        using (SqliteCommand cmd = new SqliteCommand(SQLResTableCreate, dbConn))
                        {
                            int response = cmd.ExecuteNonQuery();
                            Toast.MakeText(this, $"{response} rows affected", ToastLength.Short).Show();
                        }
                        using (SqliteCommand cmd = new SqliteCommand(SQLQuTableCreate, dbConn))
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
                //Toast.MakeText(this, "Db allready exists", ToastLength.Short).Show();
            }
        }

        public List<QuAns> GetQuestions(string SQLgetQuestionSet, string paramName, string paramValue)
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
                            while (reader.Read()) 
                            {
                                quAns = new QuAns();
                                quAns.qId = reader.GetInt32(0);
                                quAns.question = reader.GetString(1);
                                quAns.correctAnswer = reader.GetString(2);
                                quAns.wrongAnswer1 = (reader.GetString(3));
                                quAns.wrongAnswer2 = (reader.GetString(4));
                                quAns.wrongAnswer3 = (reader.GetString(5));
                                quAnsList.Add(quAns);
                            }
                        }
                    } dbConn.Close();
                }
            } catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            return quAnsList;
        }

        public void SetContent(List<QuAns> qlist, int qNumber)
        {
            if (qNumber < qlist.Count)
            {
                quAns = qlist[qNumber];
                List<string> answers = new List<string>();
                answers.Add(quAns.correctAnswer);
                answers.Add(quAns.wrongAnswer1);
                answers.Add(quAns.wrongAnswer2);
                answers.Add(quAns.wrongAnswer3);
                int qNumberToDisplay = qNumber + 1;
                txtNumber.Text = "Question number: " + qNumberToDisplay.ToString();
                txtQuestion.Text = quAns.question;
                buttonConfirm = FindViewById<Button>(Resource.Id.btnConfirm);
                spinAnswer = FindViewById<Spinner>(Resource.Id.answer);
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, answers.Shuffle());
                spinAnswer.Adapter = adapter;
            }
            else
            {
                Toast.MakeText(this, "No more questions", ToastLength.Long).Show();
            }
        }

        public void CheckAnswer(string currentAnswer, string correctAnswer)
        {
            if (currentAnswer == correctAnswer)
            {
                this.scoreList.Add(1);
            }
            else
            {
                this.scoreList.Add(0);
            }

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
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        public void FinishQuizz(List<int> scoreList)
        {
            int maxScore = scoreList.Count;
            int yourScore = scoreList.Sum();
            string totalScore = yourScore.ToString() + " / " + maxScore.ToString();
            this.score = totalScore;
            InsertResult(SQLInsertResult, "result", score);
            try
            {
                Intent intent = new Intent(this, typeof(Result));
                intent.PutExtra("totalScore", totalScore);
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
    }
}