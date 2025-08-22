using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIT.Travelnize.Domain.Base
{
    public interface IEntity
    {
        Guid Id { get; }
    }
}
