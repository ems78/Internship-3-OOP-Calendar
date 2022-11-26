using System;
using System.Collections.Generic;

namespace MailCalendar.classes
{
    internal class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; }
        public string Email { get; }
        public Dictionary<string, bool> Attendance { get; private set; }


        public Person(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Attendance = new Dictionary<string, bool>();
        }


        public void SetAttendance(string id, bool isAttending)
        {
            if (!Attendance.ContainsKey(id))
            {   
                Attendance.Add(id, isAttending);
                return;
            }
            Attendance[id] = isAttending;
        }

        
        public string GetNameSurname()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
