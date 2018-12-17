using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace JsonEmployeeGenerator
{
    class Process
    {
        const string Manager = "Manager";
        static string inputFile = ConfigurationManager.AppSettings["InputFilePath"];
        static string outputFile = ConfigurationManager.AppSettings["OutputFilePath"];

        static string[] roles = new string[] { "Junior Developer", "Semi Senior Developer", "Senior Developer", "Principal", "Team Leader" };

        static string[] teams = new string[] { "Platform", "Sales", "Billing", "Mirage" };

        public static List<Employee> ReadEmployees(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File path does not exist.");
            }

            var generator = new Random();
            var employeeData = File.ReadAllLines(filePath).ToArray();
            var employees = new List<Employee>();
            int index = 0;

            foreach (string data in employeeData)
            {
                
                string[] employeeSplitData = data.Split('\t');
                if (employeeSplitData.Length != 3)
                {
                    throw new Exception("Data is unreadable. Please check file.");
                }

                Employee employee = new Employee();
                
                employee.Id = ++index;
                employee.Name = employeeSplitData[0];
                employee.SurName = employeeSplitData[1];
                employee.Email = employeeSplitData[2];
                employee.Age = generator.Next(18, 66);
                employee.Teams = new List<string>();

                if (index < 11)
                {
                    employee.Role = Manager;
                    employees.Add(employee);
                    continue;
                }

                employee.ManagerId = generator.Next(1, 11);
                employee.Role = roles[generator.Next(5)];

                // 3 teams MAX
                int count = generator.Next(1, 4);

                for (int j = 0; j < count; j++)
                {
                    string team = teams[generator.Next(4)];
                    
                    //Check is exist in list
                    if (!employee.Teams.Contains(team))
                    {
                        employee.Teams.Add(team);
                    }
                }


                employees.Add(employee);
            }

            return employees;
        }

        public static void WriteJson(List<Employee> employees)
        {           
            StringBuilder data = new StringBuilder();

            foreach (Employee employee in employees)
            {
                data.AppendFormat("{{\"Id\":{0},\"Name\":\"{1}\",\"SurName\":\"{2}\",\"Email\":\"{3}\",\"Age\":{4},\"ManagerId\":{5},\"Teams\":[{6}],\"Role\":\"{7}\"}},",
                    employee.Id,
                    employee.Name,
                    employee.SurName,
                    employee.Age,
                    employee.Email,
                    employee.ManagerId.HasValue ? employee.ManagerId.ToString() : "null",
                    string.Join(",", employee.Teams.Select(x => "\"" + x + "\"")),
                    employee.Role
                    );
            }

            var jsonFile = File.CreateText(outputFile);
            jsonFile.WriteLine(data.Insert(0,'[').Remove(data.Length-1, 1).Append("]"));
            jsonFile.Flush();
        }
        
        public static void GenerateJSON()
        {
            List<Employee> employees = ReadEmployees(inputFile);
            WriteJson(employees);
        }
    }
}
