using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SomeCompanySite.Domain.Repositories.Abstract;

namespace SomeCompanySite.Domain
{
    public class DataManager
    {
        //Это такой, централизованый класс для работы сразу с обеими таблицами.
        //Тоесть нам не придется по отдельности создавать каждый 
        public ITextFieldsRepository TextFields { get; set; }
        public IServiceItemsRepository ServiceItems { get; set; }

        public DataManager(ITextFieldsRepository textFieldsRepository, IServiceItemsRepository serviceItemsRepository)
        {
            TextFields = textFieldsRepository;
            ServiceItems = serviceItemsRepository;
        }
    }
}
