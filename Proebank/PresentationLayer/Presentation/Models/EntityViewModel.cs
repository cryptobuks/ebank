using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Models;

namespace Presentation.Models
{
    public class EntityViewModel
    {
        public Guid Id { get; set; }
        public bool IsRemoved { get; set; }

        public EntityViewModel()
        {
        }

        public EntityViewModel(Entity entity)
        {
            Id = entity.Id;
            IsRemoved = entity.IsRemoved;
        }
    }
}