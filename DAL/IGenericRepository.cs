﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();

        Task<T> GetById(object id);

        Task<Task> Add(T obj);

        Task<Task> Update(T obj);

        Task<bool> Delete(object id);

        void Save();
    }
}
