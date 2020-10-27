using System;
using EmployeesDB.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EmployeesDB
{
    class Program
    {
        static private EmployeesContext _context = new EmployeesContext();
        static void Main(string[] args)
        {
            deleteTown("Monroe");
        }

        //Task 1
        static string GetHighlyPaidEmployees()
        {
            var employees = _context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary,
                    e.Address.AddressText,
                    e.Department.Name,
                    e.HireDate

                })
                .Where(e => e.Salary > 48000)
                .OrderBy(e => e.LastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName}, {e.JobTitle}, {e.Salary}, {e.AddressText}, department name: {e.Name}, hire date: {e.HireDate};");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 2
        static string RelocateEmployees()
        {
            Addresses address = new Addresses
            {
                TownId = 11,
                AddressText = "137 Brown St."
            };

            var adresses = _context.Addresses.Add(address);
            _context.SaveChanges();

            var SelectAdressId = _context.Addresses
                .Select(a => new
                {
                    a.AddressId,
                    a.AddressText
                })
                .Where(a => a.AddressText.Equals("137 Brown St."))
                .FirstOrDefault();

            var employees = _context.Employees
                .Include("Address")
                .Include("Department")
                .Where(e => e.LastName == "Brown");

            foreach (var e in employees)
            {
                e.AddressId = SelectAdressId.AddressId;
            }

            _context.SaveChanges();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName}, {e.JobTitle}, {e.Salary}, {e.Address.AddressText}, department name: {e.Department.Name}, hire date: {e.HireDate};");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 3
        static string ProjectsAudit()
        {
            DateTime startDdate = new DateTime(2002, 1, 1);
            DateTime endDate = new DateTime(2005, 12, 31, 23, 59, 59);

            var employeesProjects = _context.EmployeesProjects
                .Include(e => e.Employee)
                    .ThenInclude(e => e.Manager)
                .Include(e => e.Project)
                .Where(e => e.Project.StartDate >= startDdate & e.Project.EndDate <= endDate)
                .Take(5)
                .ToList();

            var sb = new StringBuilder();

            foreach(var ep in employeesProjects)
            {
                if (ep.Project.EndDate == null)
                {
                    sb.AppendLine($"Employee: {ep.Employee.FirstName} {ep.Employee.LastName} {ep.Employee.MiddleName}, " +
                    $"Manager: {ep.Employee.Manager.FirstName} {ep.Employee.Manager.LastName} {ep.Employee.Manager.MiddleName}\n" +
                    $"Project: {ep.Project.Name}, start date: {ep.Project.StartDate}, НЕ ЗАВЕРШЕН \n");
                }

                else
                {
                    sb.AppendLine($"Employee: {ep.Employee.FirstName} {ep.Employee.LastName} {ep.Employee.MiddleName}, " +
                        $"Manager: {ep.Employee.Manager.FirstName} {ep.Employee.Manager.LastName} {ep.Employee.Manager.MiddleName}\n" +
                        $"Project: {ep.Project.Name}, start date: {ep.Project.StartDate}, end date: {ep.Project.EndDate}\n");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Task 4
        static string EmployeeDossier()
        {
            Console.Write("Введите id сотрудника: ");

            int id = int.Parse(Console.ReadLine());

            var employeeProjects = _context.EmployeesProjects
                .Include(ep => ep.Employee)
                .Include(ep => ep.Project)
                .Where(ep => ep.EmployeeId == id)
                .ToList();

            var sb = new StringBuilder();

            var employee = _context.Employees
                .Where(e => e.EmployeeId == id)
                .ToList();

            foreach (var e in employee)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle}\n");
            }

            sb.AppendLine("Проекты сотрудника:");

            foreach (var ep in employeeProjects)
            {
                sb.AppendLine($"{ep.Project.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        //Task 5
        static string SmallDepartments()
        {
            var departments = _context.Departments
                .Select(d => new
                {
                    d.Name,
                    d.DepartmentId
                })
                .ToList();

            var employees = _context.Employees
                .Select(e => new
                {
                    e.DepartmentId
                })
                .ToList();

            var sb = new StringBuilder();

            foreach(var d in departments)
            {
                if(employees.Where(e => e.DepartmentId == d.DepartmentId).Count() < 5)
                {
                    sb.AppendLine($"{d.Name}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Task 6
        static void RaiseSalary(string departmentName, int percentage)
        {
            var sb = new StringBuilder();
            var employees = _context.Employees
                .Include("Department")
                .Where(e => e.Department.Name.Equals(departmentName))
                .ToList();

            sb.AppendLine("\n");

            foreach (var e in employees)
            {
                e.Salary = e.Salary + e.Salary*percentage/100;
            }

            _context.SaveChanges();
        }

        //Task 7
        static void deleteDepartment(int id)
        {
            var department = _context.Departments
                .Where(d => d.DepartmentId == id)
                .FirstOrDefault();

            _context.Departments.Remove(department);

            _context.SaveChanges();
        }

        //Task 8
        static void deleteTown(string name)
        {
            var town = _context.Towns
                .Where(t => t.Name.Equals(name))
                .FirstOrDefault();

            _context.Towns.Remove(town);

            _context.SaveChanges();
        }
    }
}
