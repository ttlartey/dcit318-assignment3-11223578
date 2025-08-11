using System;
using System.Collections.Generic;
using System.Linq;

// Generic repository class
public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item)
    {
        items.Add(item);
    }

    public List<T> GetAll()
    {
        return items;
    }

    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// Patient entity
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

// Prescription entity
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }
}

// Healthcare system application
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    // Seed initial data
    public void SeedData()
    {
        // Add patients
        _patientRepo.Add(new Patient(1, "John Doe", 30, "Male"));
        _patientRepo.Add(new Patient(2, "Jane Smith", 25, "Female"));
        _patientRepo.Add(new Patient(3, "Samuel Green", 40, "Male"));

        // Add prescriptions
        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Paracetamol", DateTime.Now.AddDays(-3)));
        _prescriptionRepo.Add(new Prescription(4, 3, "Vitamin C", DateTime.Now.AddDays(-7)));
        _prescriptionRepo.Add(new Prescription(5, 2, "Cough Syrup", DateTime.Now.AddDays(-1)));
    }

    // Build map of prescriptions grouped by patient ID
    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo.GetAll()
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    // Print all patients
    public void PrintAllPatients()
    {
        Console.WriteLine("\nAll Patients:");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
        }
    }

    // Get prescriptions for a given patient
    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        if (_prescriptionMap.ContainsKey(patientId))
        {
            return _prescriptionMap[patientId];
        }
        return new List<Prescription>();
    }

    // Print prescriptions for a given patient
    public void PrintPrescriptionsForPatient(int id)
    {
        var prescriptions = GetPrescriptionsByPatientId(id);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine($"No prescriptions found for patient ID {id}.");
            return;
        }

        Console.WriteLine($"\nPrescriptions for Patient ID {id}:");
        foreach (var p in prescriptions)
        {
            Console.WriteLine($"ID: {p.Id}, Medication: {p.MedicationName}, Date Issued: {p.DateIssued.ToShortDateString()}");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var app = new HealthSystemApp();

        app.SeedData();
        app.BuildPrescriptionMap();

        app.PrintAllPatients();

        Console.Write("\nEnter Patient ID to view prescriptions: ");
        if (int.TryParse(Console.ReadLine(), out int patientId))
        {
            app.PrintPrescriptionsForPatient(patientId);
        }
        else
        {
            Console.WriteLine("Invalid ID entered.");
        }
    }
}
