//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Android.Support.V7.App;
//using Android.Support.V4.Widget;

//namespace ZdrowiePlus
//{
//    public class MyActionBarDrawerToggle : ActionBarDrawerToggle
//    {
//        private Activity host;
//        private int openResource;
//        private int closeResource;

//        public MyActionBarDrawerToggle(Activity host, DrawerLayout drawerLayout, int openResource, int closeResource)
//            : base(host, drawerLayout, openResource, closeResource)
//        {
//            this.host = host;
//            this.openResource = openResource;
//            this.closeResource = closeResource;
//        }

//        public override void OnDrawerOpened(View drawerView)
//        {
//            base.OnDrawerOpened(drawerView);
//        }

//        public override void OnDrawerClosed(View drawerView)
//        {
//            base.OnDrawerClosed(drawerView);
//        }

//        public override void OnDrawerSlide(View drawerView, float slideOffset)
//        {
//            base.OnDrawerSlide(drawerView, slideOffset);
//        }
//    }
//}