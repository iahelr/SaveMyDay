﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaveMayDay.Common;
using SaveMyDate.Entities;

namespace SaveMyDay.ServerLogic
{
    public class ServerLogic
    {
        // Ctor
        public ServerLogic()
        {
            
        }

        // Ctor with user details for login.
        public ServerLogic(User _user)
        {

        }

        // Connect to the "system"
        static bool connect(User _user)
        {
            return false;
        }

        // Active the algoritem
        public void activate()
        {
            // Run the active methods on algoritem
        }

        // Get Recommended path by priority
        public Path getResult(int priority)
        {
            return null;
        }

        // Get accept from user of a path.
        public void accept(List<Appointment> _appointments)
        {
            // insert the appointments to the "company"
        }

    }
}
