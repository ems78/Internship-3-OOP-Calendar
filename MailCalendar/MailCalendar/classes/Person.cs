using System;
using System.Collections.Generic;

namespace MailCalendar.classes
{
    internal class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; }
        public string Email { get; }
        public Dictionary<Guid, bool> Attendance { get; private set; }


        public Person(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }


        public void SetAttendance(Guid id, bool isAttending)
        {
            if (!Attendance.ContainsKey(id))
            {   
                Attendance.Add(id, isAttending);
                return;
            }
            Attendance[id] = isAttending;
        }
    }
}
