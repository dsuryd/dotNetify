using System;
using System.Collections.Generic;
using DotNetify;

namespace ViewModels
{
   public class StudentsVM : BaseVM
   {
      public IEnumerable<Student> Students
      {
         get { return StudentEntities.GetAll; }
      }

      public Student Create
      {
         get { return null; }
         set { this.AddList(() => Students, StudentEntities.Add(value)); }
      }
   }

   public class Student
   {
      public int ID { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
   }

   public class StudentEntities
   {
      private static List<Student> mStudents = new List<Student> { new Student { ID = 1, FirstName = "John", LastName = "Smith" } };

      public static IEnumerable<Student> GetAll
      {
         get { return mStudents; }
      }

      public static Student Add(Student iStudent)
      {
         iStudent.ID = mStudents.Count + 1;
         mStudents.Add(iStudent);
         return iStudent;
      }
   }
}