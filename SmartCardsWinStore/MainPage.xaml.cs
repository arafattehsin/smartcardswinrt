using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Helpers;
using Windows.System.Threading;
using SmartCardLibrary;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartCardsWinStore
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region SmartCard Helper Fields.
        SmartCardHelper smartCard;
        IAsyncAction ThreadPoolWorkItem;
        bool isVerifiedFlag = false;
        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            this.InitializeCardHelper();
        }

        #region Smart Card Reader Methods.
        private void InitializeCardHelper()
        {
            smartCard = new SmartCardHelper();
            isVerifiedFlag = true;

            if ((smartCard.Readers.Count() > 0))
            {
                ThreadPoolWorkItem = Windows.System.Threading.ThreadPool.RunAsync(
               (source) =>
               {
                   while (isVerifiedFlag)
                   {
                       if (source.Status == AsyncStatus.Canceled)
                       {
                           break;
                       }

                       if (isVerifiedFlag)
                       {
                           // Only update if the progress value has changed.
                           CheckUpdatedStatusWithHelper();

                       }
                   }
               },
           WorkItemPriority.Normal);

            }

        }
        private async void CheckUpdatedStatusWithHelper()
        {
            while (isVerifiedFlag)
            {
                SmartcardErrorCode result;

                lock (smartCard)
                {
                    if (!smartCard.HasContext)
                    {
                        return;
                    }

                    result = smartCard.GetStatusChange(smartCard);
                }

                if ((result == SmartcardErrorCode.Timeout))
                {
                    // Time out has passed, but there is no new info. Just go on with the loop
                    continue;
                }

                for (int i = 0; i <= smartCard.Readers.Length - 1; i++)
                {


                    //Check if the state changed from the last time.
                    if ((smartCard.Readers[i].EventState & CardState.Changed) == CardState.Changed)
                    {
                        //Check what changed.
                        SmartcardState state = SmartcardState.None;
                        if ((smartCard.Readers[i].EventState & CardState.Present) == CardState.Present
                            && (smartCard.Readers[i].CurrentState & CardState.Present) != CardState.Present)
                        {
                            //The card was inserted.                            
                            state = SmartcardState.Inserted;
                        }
                        else if ((smartCard.Readers[i].EventState & CardState.Empty) == CardState.Empty
                            && (smartCard.Readers[i].CurrentState & CardState.Empty) != CardState.Empty)
                        {
                            //The card was ejected.
                            state = SmartcardState.Ejected;
                        }
                        if (state != SmartcardState.None && smartCard.Readers[i].CurrentState != CardState.None)
                        {
                            switch (state)
                            {
                                case SmartcardState.Inserted:
                                    {
                                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                        {
                                            bool isConnected = smartCard.Connect(smartCard, smartCard.Readers[0].Reader);
                                            CardIDBlock.Text = smartCard.GetCardID(smartCard);
                                        });

                                        break;
                                    }
                                case SmartcardState.Ejected:
                                    {
                                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                        {
                                            CardIDBlock.Text = "Card Ejected";
                                        });
                                        
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                        }

                        smartCard.Readers[i].CurrentState = smartCard.Readers[i].EventState;
                    }
                }
            }
        }

        #endregion
    }
}
