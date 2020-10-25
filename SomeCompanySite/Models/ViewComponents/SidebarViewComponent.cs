using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SomeCompanySite.Domain;

namespace SomeCompanySite.Models.ViewComponents
{
    //Для того чтобы класс стал Вью компонентом, надо унаследоваться от ViewComponent
    public class SidebarViewComponent : ViewComponent
    {
        private readonly DataManager dataManager;

        public SidebarViewComponent( DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        //InvokeAsync и представление default имеют такие названия, потому что так надо!
        //А вообще весь этот компонент, нам нужен для того чтобы не менять для каждой странички контроллеры, а сделать сайдбар отдельным
        //(Так как там будет взаимодействие с данными, контроллеры пришлось бы дополнять)
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult) View("Default", dataManager.ServiceItems.GetServiceItems()));
        }
    }
}
