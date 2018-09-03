using System;
using System.Collections.Generic;
using DotNetify.Elements;

namespace DotNetify.DevApp
{
   public class DataFlow : BaseVM
   {
      public string Content => new Markdown("DotNetify.DevApp.Docs.DataFlow.md");
   }

   public class MasterDetails : BaseVM
   {
      private readonly IWebStoreService _webStoreService;

      private event EventHandler<int> SelectedItem;

      public MasterDetails(IWebStoreService webStoreService)
      {
         _webStoreService = webStoreService;
      }

      public override void OnSubVMCreated(BaseVM vm)
      {
         if (vm is MasterList)
         {
            var masterList = vm as MasterList;
            masterList.ListItems = _webStoreService.GetAllBooks();
            masterList.Selected += (sender, id) => SelectedItem?.Invoke(this, id);
         }
         else if (vm is Details)
         {
            var details = vm as Details;
            SelectedItem += (sender, id) => details.SetData(_webStoreService.GetBookById(id));
         }
      }
   }

   public class MasterList : BaseVM
   {
      public IEnumerable<WebStoreRecord> ListItems
      {
         get => Get<IEnumerable<WebStoreRecord>>();
         set => Set(value);
      }

      public event EventHandler<int> Selected;

      public Action<int> Select => id => Selected?.Invoke(this, id);
   }

   public class Details : BaseVM
   {
      public string ItemImageUrl
      {
         get => Get<string>();
         set => Set(value);
      }

      public void SetData(WebStoreRecord data) => ItemImageUrl = data.ImageUrl;
   }
}