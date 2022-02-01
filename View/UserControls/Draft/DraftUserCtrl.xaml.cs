using ShopUrban.Model;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopUrban.View.UserControls.Draft
{
    /// <summary>
    /// Interaction logic for DraftUserCtrl.xaml
    /// </summary>
    public partial class DraftUserCtrl : UserControl
    {
        public List<CartDraft> cartDrafts
        {
            get { return (List<CartDraft>)GetValue(cartDraftsProperty); }
            set { SetValue(cartDraftsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for cartDrafts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty cartDraftsProperty =
            DependencyProperty.Register("cartDrafts", typeof(List<CartDraft>), 
                typeof(DraftItemUserCtrl), new PropertyMetadata(null));

        public DraftUserCtrl()
        {
            InitializeComponent();

            DataContext = this;

            this.Loaded += UserControl_Loaded;

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Background = Brushes.Transparent;
            }

            MyEventBus.subscribe(handleEvent);
        }

        void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += window_Closing;
        }

        void window_Closing(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            //do something before the window is closed...
            MyEventBus.unSubscribe(handleEvent);
        }

        public void refresh()
        {
            cartDrafts = CartDraft.all();
        }

        private void handleEvent(EventMessage eventMessage)
        {
            switch (eventMessage.eventId)
            {
                case EventMessage.EVENT_DRAFT_DELETED:
                    refresh();
                    break;

                default:
                    break;
            }
        }

    }
}
