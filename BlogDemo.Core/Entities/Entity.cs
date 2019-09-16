using BlogDemo.Core.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Core.Entities
{
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }
    }
}
