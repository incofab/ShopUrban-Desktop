using ShopUrban.Model;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for DraftItemUserCtrl.xaml
    /// </summary>
    public partial class DraftItemUserCtrl : UserControl
    {
        public CartDraft cartDraft
        {
            get { return (CartDraft)GetValue(cartDraftProperty); }
            set { SetValue(cartDraftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for cartDraft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty cartDraftProperty =
            DependencyProperty.Register("cartDraft", typeof(CartDraft), typeof(DraftItemUserCtrl), new PropertyMetadata(null));

        public DraftItemUserCtrl()
        {
            InitializeComponent();

            
        }

        private void updateUI()
        {
            //Trace.WriteLine("cartDraft.quantity = " + cartDraft.quantity);
            //Trace.WriteLine("cartDraft.cartItems.Count = " + cartDraft.cartItems.Count);

            tbAmount.Text = Helpers.naira(cartDraft.amount);
            tbDate.Text = cartDraft.created_at;
            tbQuantity.Text = cartDraft.quantity + " items";
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if ("cartDraft".Equals(e.Property.ToString()))
            {
                updateUI();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            MyEventBus.post(new EventMessage(EventMessage.EVENT_DRAFT_EDIT, cartDraft));
            
            TimerHelper.SetTimeout(1000, () =>
            {

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {

                    deleteCartDraft(cartDraft);

                }));

            });
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to delete this draft?", "Delete Draft", MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes) return;

            deleteCartDraft(cartDraft);
        }

        private void deleteCartDraft(CartDraft cartDraft)
        {
            CartDraft.delete(cartDraft);
            MyEventBus.post(new EventMessage(EventMessage.EVENT_DRAFT_DELETED, cartDraft));
        }
    }
}
