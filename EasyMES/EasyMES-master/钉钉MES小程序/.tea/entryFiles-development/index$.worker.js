/**! __CODEPLACEHOLDER_START__ */ /*[PositionForHostEntryCodeBegin]*/ /**! __CODEPLACEHOLDER_END__ */
if(!self.__appxInited) {
self.__appxInited = 1;


require('./config$');
require('./importScripts$');

      function getUserAgentInPlatformWeb() {
        return typeof navigator !== 'undefined' ? navigator.swuserAgent || navigator.userAgent || '' : '';
      }
      if(getUserAgentInPlatformWeb() && (getUserAgentInPlatformWeb().indexOf('LyraVM') > 0 || getUserAgentInPlatformWeb().indexOf('AlipayIDE') > 0) ) {
        var AFAppX = self.AFAppX.getAppContext ? self.AFAppX.getAppContext().AFAppX : self.AFAppX;
      } else {
        importScripts('https://appx/af-appx.worker.min.js');
        var AFAppX = self.AFAppX;
      }
      self.getCurrentPages = AFAppX.getCurrentPages;
      self.getApp = AFAppX.getApp;
      self.Page = AFAppX.Page;
      self.App = AFAppX.App;
      self.my = AFAppX.bridge || AFAppX.abridge;
      self.abridge = self.my;
      self.Component = AFAppX.WorkerComponent || function(){};
      self.$global = AFAppX.$global;
      self.requirePlugin = AFAppX.requirePlugin;
    

if(AFAppX.registerApp) {
  AFAppX.registerApp({
    appJSON: appXAppJson,
  });
}



function success() {
require('../../app');
require('../../node_modules/mini-ali-ui/es/am-icon/index?hash=05d2a9730dd6009bf9446182f9c985f40f8c0f43');
require('../../components/table/table?hash=05d2a9730dd6009bf9446182f9c985f40f8c0f43');
require('../../node_modules/mini-ali-ui/es/badge/index?hash=05d2a9730dd6009bf9446182f9c985f40f8c0f43');
require('../../node_modules/mini-ali-ui/es/tabs/index?hash=4a98b35daa3eee1b62960d4d5bbcbf254cbf11e3');
require('../../node_modules/mini-ali-ui/es/tabs/tab-content/index?hash=05d2a9730dd6009bf9446182f9c985f40f8c0f43');
require('../../node_modules/mini-ali-ui/es/popup/index?hash=05d2a9730dd6009bf9446182f9c985f40f8c0f43');
require('../../node_modules/mini-ali-ui/es/search-bar/index?hash=5a0c180d5ccf7c9d483dd4817cdab5489824013c');
require('../../components/cascadeSelect/cascadeSelect?hash=a7c7ff94575e6a74a8436ca493f3d7fc35d9bef5');
require('../../pages/index/index?hash=9d83a615ce5b871850ea4f92cabb2c24dc7e93c1');
require('../../pages/InStorage/InStorageBanding/InStorageBanding?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/InStorage/InStorageCancleBanding/InStorageCancleBanding?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/InStorage/InStorageLocation/InStorageLocation?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Storage/StorageChange/StorageChange?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/OutStorage/OutStorage/OutStorage?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/OutStorage/OutStorageBanding/OutStorageBanding?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/OutStorage/OutStorageCancle/OutStorageCancle?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/TaskJob/DoTaskJob/DoTaskJob?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/TaskJob/MyTaskJob/MyTaskJob?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/User/Login/Login?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/User/Setting/Setting?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/NeedMaterialJob/NeedMaterialJob?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/ConsumeMaterial/ConsumeMaterial?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/ReturnMaterial/ReturnMaterial?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/ReturnMaterialJob/ReturnMaterialJob?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/OutputMaterial/OutputMaterial?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/OutputMaterialCancle/OutputMaterialCancle?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/OutputMaterialLocation/OutputMaterialLocation?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/FinishWorkOrderJob/FinishWorkOrderJob?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/User/DingBanding/DingBanding?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Storage/StorageMove/StorageMove?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Storage/GetStorage/GetStorage?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Storage/AreaStorage/AreaStorage?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Checkout/CheckoutInfo/CheckoutInfo?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/GetWorkOrderInfo/GetWorkOrderInfo?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Hotline/HotlineParam/HotlineParam?hash=10b72c379c9ef4bde892bb6d1de496f4c1ccfb26');
require('../../pages/Checkout/TwoCheckout/TwoCheckout?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Checkout/CheckoutJob/CheckoutJob?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Checkout/CheckoutCancle/CheckoutCancle?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Checkout/FirstCheckout/FirstCheckout?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Checkout/ReturnCheckout/ReturnCheckout?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Checkout/ChangeCheckoutState/ChangeCheckoutState?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Checkout/AddScapImage/AddScapImage?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/EqpRepair/EqpCallRepair/EqpCallRepair?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/EqpRepair/EqpRepairInfo/EqpRepairInfo?hash=84ab3c59a0b72e3104e9cc541477ed9d8e62d692');
require('../../pages/EqpRepair/EqpRepairDetail/EqpRepairDetail?hash=a18afee1314913a79bb19a208f52b8945e88da70');
require('../../pages/EqpRepair/EqpRepairRecord/EqpRepairRecord?hash=0ea9c4d734f7c087f926f15212691d0bb44f8cf9');
require('../../pages/EqpRepair/EqpRepairFlow/EqpRepairFlow?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/Checkout/BadStorageInfo/BadStorageInfo?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/OutputMaterialLocation2/OutputMaterialLocation2?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/WorkRun/ConsumeMaterial2/ConsumeMaterial2?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
require('../../pages/OutStorage/OutStorage2/OutStorage2?hash=32d7d2807ed4e666ef03b4b3fe8c38ecf2e34e68');
}
self.bootstrapApp ? self.bootstrapApp({ success }) : success();
}