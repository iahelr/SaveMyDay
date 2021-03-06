﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveMyDate.Entities
{
    public class DbAppointmentCompany : IMongoEntity
    {
        public string Id { get; set; }
        public Company Company { get; set; }
        public List<DbAppointment> freeAppointments { get; set; }

        public DbAppointmentCompany()
        {
            this.freeAppointments = new List<DbAppointment>();
        }

        public List<Appointment> ConvertToAppointments()
        {
            List<Appointment> appointments = new List<Appointment>();

            foreach(DbAppointment dbAppointment in this.freeAppointments)
            {
                Appointment app = new Appointment();
                app.Company = this.Company;
                app.Remark = dbAppointment.Remark;
                app.Time = dbAppointment.StartTime;
                appointments.Add(app);
            }

            return appointments;
        }
    }
}
