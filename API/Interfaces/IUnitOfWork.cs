using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces;

    public interface IUnitOfWork
    {
        IMemberRepository memberRepository{get;}
        
        Task<bool> Complete();
        bool HasChanges();
    }
