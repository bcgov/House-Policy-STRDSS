using Interop.UIAutomationClient;
using Interop.UIAutomationCore;

namespace TestFrameWork.WindowsAutomation.Controls
{
    public class FileDialog
    {

        public void FindAndSet(string FileName, string WindowName, string AutomationID)
        {

            //WindowName = open, class name = #32770
            var automation = new CUIAutomation();
            IUIAutomationElement desktop = automation.GetRootElement();
            IUIAutomationCondition NameCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, WindowName);
            //IUIAutomationCondition condition = automation.CreatePropertyCondition(UIA_PropertyIds.uia, WindowName);
            IUIAutomationCondition ClassCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, WindowName);

            IUIAutomationElement fileDialog = desktop.FindFirst(TreeScope.TreeScope_Children, NameCondition);
            if (fileDialog != null)
            {
                //className = edit, automationID= 1148
                IUIAutomationCondition fileNameCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_AutomationIdPropertyId, AutomationID);
                IUIAutomationElement fileNameBox = fileDialog.FindFirst(TreeScope.TreeScope_Descendants, fileNameCondition);
                if (fileNameBox != null)
                {
                    var valuePattern = (IUIAutomationValuePattern)fileNameBox.GetCurrentPattern(UIA_PatternIds.UIA_ValuePatternId);
                    valuePattern.SetValue(FileName);
                }

                IUIAutomationCondition openButtonCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, WindowName);
                IUIAutomationElement openButton = fileDialog.FindFirst(TreeScope.TreeScope_Descendants, openButtonCondition);
                if (openButton != null)
                {
                    var invokePattern = (IUIAutomationInvokePattern)openButton.GetCurrentPattern(UIA_PatternIds.UIA_InvokePatternId);
                    invokePattern.Invoke();
                }
            }
        }
    }
}
