using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Student
{
    public string Name { get; }
    public string Zone { get; }

    public Student(string name, string zone)
    {
        Name = name;
        Zone = zone;
    }

    public override string ToString() => $"{Name} ({Zone})";
}

public class Bus
{
    public int Id { get; }
    public string Zone { get; }
    public int Capacity { get; }
    public List<Student> AssignedStudents { get; }

    public Bus(int id, string zone, int capacity)
    {
        Id = id;
        Zone = zone;
        Capacity = capacity;
        AssignedStudents = new List<Student>();
    }

    public bool TryAdd(Student student)
    {
        if (AssignedStudents.Count >= Capacity)
            return false;

        AssignedStudents.Add(student);
        return true;
    }

    public override string ToString()
    {
        return $"Bus {Id} (Zone {Zone}, Cap {Capacity}) - {AssignedStudents.Count} assigned";
    }
}

public class BusAllocator
{
    public List<Bus> Buses { get; }
    public List<Student> Unassigned { get; }

    public BusAllocator(List<Bus> buses)
    {
        Buses = buses;
        Unassigned = new List<Student>();
    }

    public void AssignStudents(List<Student> students)
    {
        foreach (var zoneGroup in students.GroupBy(s => s.Zone))
        {
            var zone = zoneGroup.Key;
            var studentsInZone = new Queue<Student>(zoneGroup);

            foreach (var bus in Buses.Where(b => b.Zone == zone))
            {
                while (studentsInZone.Count > 0 && bus.TryAdd(studentsInZone.Peek()))
                {
                    studentsInZone.Dequeue();
                }
            }

            foreach (var student in studentsInZone)
            {
                Unassigned.Add(student);
            }
        }

        var stillUnassigned = new List<Student>(Unassigned);
        Unassigned.Clear();

        foreach (var student in stillUnassigned)
        {
            bool assigned = false;
            foreach (var bus in Buses)
            {
                if (bus.TryAdd(student))
                {
                    assigned = true;
                    break;
                }
            }
            if (!assigned)
                Unassigned.Add(student);
        }
    }

    public void PrintManifests()
    {
        foreach (var bus in Buses)
        {
            Console.WriteLine(bus);
            foreach (var student in bus.AssignedStudents)
                Console.WriteLine($"  - {student}");
        }

        Console.WriteLine("\nUnassigned Students:");
        if (Unassigned.Count == 0)
            Console.WriteLine("  None");
        else
            foreach (var student in Unassigned)
                Console.WriteLine($"  - {student}");
    }
}
namespace _35__School_Bus_Route_Optimizer__Zones_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var buses = new List<Bus>
        {
            new Bus(1, "North", 2),
            new Bus(2, "South", 2),
            new Bus(3, "East", 2)
        };

            var students = new List<Student>
        {
            new Student("Alice", "North"),
            new Student("Bob", "North"),
            new Student("Charlie", "North"),
            new Student("David", "South"),
            new Student("Eve", "East"),
            new Student("Frank", "South"),
            new Student("Grace", "East"),
            new Student("Heidi", "North")
        };

            var allocator = new BusAllocator(buses);
            allocator.AssignStudents(students);
            allocator.PrintManifests();
        }
    }
}
