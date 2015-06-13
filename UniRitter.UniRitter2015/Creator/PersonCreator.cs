using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniRitter.UniRitter2015.Models.PersonModel;

namespace UniRitter.UniRitter2015.Creator
{
    public interface PersonCreator
    {
        public PersonModel add(PersonModel person);
    }
}
