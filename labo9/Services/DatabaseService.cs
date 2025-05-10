using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using labo9.Models;
using System.IO;
using System.Text;

namespace labo9.Services
{
    public class DatabaseService
    {
        private const string ConnectionString = "Data Source=exam_requests.db";

        public DatabaseService()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"CREATE TABLE IF NOT EXISTS ExamRequests (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId TEXT,
                FullName TEXT,
                SemesterYear TEXT,
                Major TEXT,
                Subject TEXT,
                ECTSPoints INTEGER,
                Instructor TEXT,
                Justification TEXT,
                SubmissionDate TEXT,
                StudentSignature TEXT,
                CommissionMember1 TEXT,
                CommissionMember2 TEXT,
                CommissionMember3 TEXT,
                DeanDecision TEXT,
                DecisionDate TEXT
            );";
            command.ExecuteNonQuery();
        }

        public void SaveRequest(ExamRequest request)
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"INSERT INTO ExamRequests (
                    StudentId, FullName, SemesterYear, Major, Subject,
                    ECTSPoints, Instructor, Justification, SubmissionDate,
                    StudentSignature, CommissionMember1, CommissionMember2,
                    CommissionMember3, DeanDecision, DecisionDate
                ) VALUES (
                    @StudentId, @FullName, @SemesterYear, @Major, @Subject,
                    @ECTSPoints, @Instructor, @Justification, @SubmissionDate,
                    @StudentSignature, @CommissionMember1, @CommissionMember2,
                    @CommissionMember3, @DeanDecision, @DecisionDate
                );";

                command.Parameters.AddWithValue("@StudentId", request.StudentId);
                command.Parameters.AddWithValue("@FullName", request.FullName);
                command.Parameters.AddWithValue("@SemesterYear", request.SemesterYear);
                command.Parameters.AddWithValue("@Major", request.Major);
                command.Parameters.AddWithValue("@Subject", request.Subject);
                command.Parameters.AddWithValue("@ECTSPoints", request.ECTSPoints);
                command.Parameters.AddWithValue("@Instructor", request.Instructor);
                command.Parameters.AddWithValue("@Justification", request.Justification ?? "");
                command.Parameters.AddWithValue("@SubmissionDate", request.SubmissionDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@StudentSignature", request.StudentSignature);
                command.Parameters.AddWithValue("@CommissionMember1", request.CommissionMember1);
                command.Parameters.AddWithValue("@CommissionMember2", request.CommissionMember2);
                command.Parameters.AddWithValue("@CommissionMember3", request.CommissionMember3);
                command.Parameters.AddWithValue("@DeanDecision", request.DeanDecision ?? "");
                command.Parameters.AddWithValue("@DecisionDate", request.DecisionDate.ToString("yyyy-MM-dd"));

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas zapisu do bazy danych: " + ex.Message);
            }
        }
        
         public void ExportToCsv(string filePath)
    {
        var requests = GetAllRequests();
        var sb = new StringBuilder();

        sb.AppendLine("StudentId,FullName,SemesterYear,Major,Subject,ECTSPoints,Instructor,Justification,StudentSignature,CommissionMember1,CommissionMember2,CommissionMember3,DeanDecision");

        foreach (var r in requests)
        {
            sb.AppendLine($"{r.StudentId},{r.FullName},{r.SemesterYear},{r.Major},{r.Subject},{r.ECTSPoints},{r.Instructor},{r.Justification.Replace(",", " ")},{r.StudentSignature},{r.CommissionMember1},{r.CommissionMember2},{r.CommissionMember3},{r.DeanDecision}");
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

        public void ImportFromCsv(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("Nie znaleziono pliku", filePath);

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            if (lines.Length < 2) return; 

            for (int i = 1; i < lines.Length; i++) 
            {
                var fields = lines[i].Split(',');

                if (fields.Length < 14) continue; 

                var request = new ExamRequest
                {
                    StudentId = fields[0],
                    FullName = fields[1],
                    SemesterYear = fields[2],
                    Major = fields[3],
                    Subject = fields[4],
                    ECTSPoints = int.TryParse(fields[5], out var ects) ? ects : 0,
                    Instructor = fields[6],
                    Justification = fields[7],
                    StudentSignature = fields[8],
                    CommissionMember1 = fields[9],
                    CommissionMember2 = fields[10],
                    CommissionMember3 = fields[11],
                    DeanDecision = fields[12],
                    SubmissionDate = DateTime.Now,
                    DecisionDate = DateTime.Now
                };

                SaveRequest(request);
            }
        }
        public List<ExamRequest> GetAllRequests()
        {
            var results = new List<ExamRequest>();
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ExamRequests";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new ExamRequest
                {
                    Id = reader.GetInt32(0),
                    StudentId = reader.GetString(1),
                    FullName = reader.GetString(2),
                    SemesterYear = reader.GetString(3),
                    Major = reader.GetString(4),
                    Subject = reader.GetString(5),
                    ECTSPoints = reader.GetInt32(6),
                    Instructor = reader.GetString(7),
                    Justification = reader.GetString(8),
                    SubmissionDate = DateTime.TryParse(reader.GetString(9), out var subDate) ? subDate : DateTime.MinValue,
                    StudentSignature = reader.GetString(10),
                    CommissionMember1 = reader.GetString(11),
                    CommissionMember2 = reader.GetString(12),
                    CommissionMember3 = reader.GetString(13),
                    DeanDecision = reader.GetString(14),
                    DecisionDate = DateTime.TryParse(reader.GetString(15), out var decDate) ? decDate : DateTime.MinValue
                });
            }
            return results;
        }
    }
}
