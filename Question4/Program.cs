using System;
using System.IO;
using System.Collections.Generic;

class Student
{
    public string FullName { get; set; } = "";
    public int Age { get; set; }
    public string Program { get; set; } = "";
    public double Score { get; set; }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

class Program
{
    static void Main()
    {
        string filePath = "students.txt";

        string[] sampleStudents = {
            "John Doe,20,Computer Science,80",
            "Jane Smith,22,Information Technology,85",
            "Michael Johnson,19,Software Engineering,78",
            "Emily Davis,21,Information Systems,92"
        };

        bool recreateFile = false;

        if (!File.Exists(filePath))
        {
            recreateFile = true;
        }
        else
        {
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (line.Split(',').Length != 4)
                {
                    recreateFile = true;
                    break;
                }
            }
        }

        if (recreateFile)
        {
            Console.WriteLine($"Creating or replacing '{filePath}' with sample data...");
            File.WriteAllLines(filePath, sampleStudents);
        }

        List<Student> students = new List<Student>();
        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(',');

            if (parts.Length != 4)
            {
                Console.WriteLine($"Skipping invalid line: {line}");
                continue;
            }

            if (!int.TryParse(parts[1], out int age) || !double.TryParse(parts[3], out double score))
            {
                Console.WriteLine($"Skipping line with invalid data types: {line}");
                continue;
            }

            students.Add(new Student
            {
                FullName = parts[0],
                Age = age,
                Program = parts[2],
                Score = score
            });
        }

        string reportFile = "report.txt";
        using (var writer = new StreamWriter(reportFile))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} ({student.Age}) - {student.Program}: Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }

        Console.WriteLine($"Report saved to {reportFile}");
    }
}
