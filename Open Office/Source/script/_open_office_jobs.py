import uno
import unohelper

import logging
import urllib2
import webbrowser
import os
import locale
import threading
import json
import time

from com.sun.star.task import XJob
from com.sun.star.task import XJobExecutor
from com.sun.star.beans import PropertyValue
from com.sun.star.ui import XContextMenuInterceptor
from com.sun.star.awt.PosSize import POSSIZE
from com.sun.star.awt import XActionListener


def getConfigSetting (sNodeConfig, bUpdate):
    # get a configuration node
    # example: aSettings = getConfigSetting("/org.openoffice.Office.Common/Path/Current", false)
    xSvMgr = uno.getComponentContext().ServiceManager
    xConfigProvider = xSvMgr.createInstanceWithContext("com.sun.star.configuration.ConfigurationProvider", uno.getComponentContext())
    xPropertyValue = PropertyValue()
    xPropertyValue.Name = "nodepath"
    xPropertyValue.Value = sNodeConfig
    if bUpdate:
        sService = "com.sun.star.configuration.ConfigurationUpdateAccess"
    else:
        sService = "com.sun.star.configuration.ConfigurationAccess"
    return xConfigProvider.createInstanceWithArguments(sService, (xPropertyValue,))


class ContextMenuInterceptor(unohelper.Base, XContextMenuInterceptor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()

    def notifyContextMenuExecute(self, xEvent):

        try:
            xContextMenu = xEvent.ActionTriggerContainer
            if xContextMenu:

                menuItemBuilder = MenuItemBuilder(xContextMenu)

                desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)

                document = desktop.getCurrentComponent()

                url, color = SelectionHelper.get_url_and_color(document)

                container_text = 'x'
                command_national_case_law = 'x'
                command_national_legislation = 'x'
                command_eu_case_law = 'x'
                command_eu_legislation = 'x'
                command_all = 'x'

                if color == Constant.Green_Color:
                    container_text = Constant.UI_ContextMenu_EN_ReferingActOrProvision
                    command_national_case_law = Constant.Command_Ref_RelevantNationalCaseLaw
                    command_national_legislation = Constant.Command_Ref_RelevantNationalLegislation
                    command_eu_case_law = Constant.Command_Ref_RelevantEuCase
                    command_eu_legislation = Constant.Commanad_Ref_RelevantEuLegislation
                    command_all = Constant.Command_Ref_AllRelevant

                elif color == Constant.Red_Color:
                    container_text = Constant.UI_ContextMenu_EN_IndexedTerms
                    command_national_case_law = Constant.Command_Term_RelevantNationalCaseLaw
                    command_national_legislation = Constant.Command_Term_RelevantNationalLegislation
                    command_eu_case_law = Constant.Command_Term_RelevantEuCaseLaw
                    command_eu_legislation = Constant.Command_Term_RelevantEULegislation
                    command_all = Constant.Command_Term_AllRelevant

                if (color == Constant.Green_Color or color == Constant.Red_Color) and url:
                    i = xContextMenu.Count

                    # line separator
                    separator = xContextMenu.createInstance("com.sun.star.ui.ActionTriggerSeparator")
                    separator.SeparatorType = uno.getConstantByName("com.sun.star.ui.ActionTriggerSeparatorType.LINE")
                    xContextMenu.insertByIndex(i, separator)
                    i = i + 1


                    eurocasesServiceMenuContainer = xContextMenu.createInstance("com.sun.star.ui.ActionTriggerContainer")

                    eurocasesServiceMenuItem = xContextMenu.createInstance("com.sun.star.ui.ActionTrigger")
                    eurocasesServiceMenuItem.setPropertyValue("Text", container_text)
                    eurocasesServiceMenuItem.setPropertyValue("SubContainer", eurocasesServiceMenuContainer)

                    nationalCaseLawMenuItem = (menuItemBuilder
                                               .with_text(Constant.UI_ContextMenu_EN_NationalCaseLaw)
                                               .with_command(command_national_case_law)
                                               .result())

                    nationalLegislationMenuItem = (menuItemBuilder
                                                   .with_text(Constant.UI_ContextMenu_EN_NationalLegislation)
                                                   .with_command(command_national_legislation)
                                                   .result())

                    euLawMenuItem = (menuItemBuilder
                                     .with_text(Constant.UI_ContextMenu_EN_EUCaseLaw)
                                     .with_command(command_eu_case_law)
                                     .result())

                    euLegislationMenuItem = (menuItemBuilder
                                             .with_text(Constant.UI_ContextMenu_EN_EULegislation)
                                             .with_command(command_eu_legislation)
                                             .result())

                    allMenuItem = (menuItemBuilder
                                   .with_text(ResourceManager.all())
                                   .with_command(command_all)
                                   .result())

                    eurocasesServiceMenuContainer.insertByIndex(0, nationalCaseLawMenuItem)
                    eurocasesServiceMenuContainer.insertByIndex(1, nationalLegislationMenuItem)
                    eurocasesServiceMenuContainer.insertByIndex(2, euLawMenuItem)
                    eurocasesServiceMenuContainer.insertByIndex(3, euLegislationMenuItem)
                    eurocasesServiceMenuContainer.insertByIndex(4, allMenuItem)

                    xContextMenu.insertByIndex(i, eurocasesServiceMenuItem)

                    if color == Constant.Green_Color:

                        showHintMenuEntry = (menuItemBuilder
                                             .with_text(ResourceManager.show_tooltip())
                                             .with_command(Constant.Command_Hint_Show)
                                             .result())

                        get_long_citate_entry = (menuItemBuilder
                                                 .with_text(ResourceManager.long_citation())
                                                 .with_command(Constant.Command_Long_Citation)
                                                 .result())

                        get_short_citate_entry = (menuItemBuilder
                                                  .with_text(ResourceManager.short_citation())
                                                  .with_command(Constant.Command_Short_Citation)
                                                  .result())
                        i += 1
                        xContextMenu.insertByIndex(i, showHintMenuEntry)

                        i += 1
                        xContextMenu.insertByIndex(i, get_long_citate_entry)

                        i += 1
                        xContextMenu.insertByIndex(i, get_short_citate_entry)

                    i += 1
                    removeMenuItem = (menuItemBuilder
                                      .with_text(Constant.UI_ContextMenu_EN_Remove)
                                      .with_command(Constant.Command_Remove_Hyperlink)
                                      .result())

                    xContextMenu.insertByIndex(i, removeMenuItem)

                return uno.getConstantByName("com.sun.star.ui.ContextMenuInterceptorAction.EXECUTE_MODIFIED")
        except Exception, e:
            LogManager.logger.exception(e)
        return uno.getConstantByName("com.sun.star.ui.ContextMenuInterceptorAction.IGNORED")


class AddLinksJob(unohelper.Base, XJobExecutor):
    # constants
    _tmp_html_file = 'file:///C:/Windows/Temp/ec-tmp.html'
    _tmp_doc_file = 'file:///C:/Users/oreshenski/Source/Python/EuLinksChecker/Extensions/ec-tmp.doc'
    _linking_service_url = 'http://techno.eucases.eu/FrontEndREST/api/'
    # _linking_service_url = 'http://localhost:60707/api'

    # properties
    _linking_service = None
    _is_linking = False

    def __init__(self, ctx):
        LogManager.init()
        try:

            self.ctx = ctx
            self._linking_service = LinkingService(AddLinksJob._linking_service_url)
        except Exception, e:
            LogManager.logger.info('\n')
            LogManager.logger.exception(e)

    def trigger(self, args):
        try:
            msg_box = (ControlBuilder(self.ctx)
                       .msg_box()
                       .with_button_type(2)
                       .with_title(Constant.Warnning)
                       .with_label(ResourceManager.confirm_linkig())
                       .with_msg_box_type('WARNINGBOX')
                       .result())

            msg_result = msg_box.execute()
            if msg_result == 1:
                AddLinksJob._is_linking = True

                # thr_progrss.daemon = True
                # thr_progrss.start()
                # self.show_progress()

                # LogManager.logger.info('#befour linking')

                # dialog = self.show_progress()

                # LogManager.logger.info('#get dialog')

                # thr_progrss = threading.Thread(target=self.show_p, args=(dialog,))
                # thr_progrss.daemon = True
                # thr_progrss.start()

                # LogManager.logger.info('#start show')

                self.link_document()

                # LogManager.logger.info('#after linking')

                # dialog.setVisible(False)
                # dialog.dispose()
                AddLinksJob._is_linking = False
                # dialog.setVisible(False)
                # dialog.dispose()
                # thr_linking = threading.Thread(target=self.link_document, args=(), kwargs={})


                #thr_linking.start()
                #thr_linking.join()
                # thr_progrss.start()

        except Exception, e:
            LogManager.logger.info('\n')
            LogManager.logger.exception(e)

            msg_box = (ControlBuilder(self.ctx)
                       .msg_box()
                       .with_button_type(1)
                       .with_title('Error')
                       .with_label(ResourceManager.error())
                       .with_msg_box_type('ERRORBOX')
                       .result())

            msg_box.execute()

    def link_document(self):
        try:
            desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)
            document = desktop.getCurrentComponent()
            # save the current document as HTML
            # AddLinksJob.store_as_html(self._tmp_html_file, document)
            FileHelper.store_as_html(Constant.Tmp_Html_File, document)
            # FileHelper.store_as_html('file:///C:/Windows/Temp/ec-tmp store.html', document)
            # read the html content
            # html = AddLinksJob.read_file(AddLinksJob._tmp_html_file

            html = FileHelper.read_file(Constant.Tmp_Html_File_Read)

            # FileHelper.write_file('C:\\Windows\\Temp\\ec-tmp-readed.html', html)

            # send the html to the linking service
            linkedHtml = self._linking_service.put_links_for_html(html)

            # FileHelper.write_file('C:\\Windows\\Temp\\ec-tmp-linked.html', linkedHtml)

            # store the linked html in the tmp file
            # AddLinksJob.write_file(AddLinksJob._tmp_html_file, linkedHtml)
            FileHelper.write_file(Constant.Tmp_Html_File_Read, linkedHtml)
            # store the linked html as doc file
            # AddLinksJob.store_as_doc(AddLinksJob.read_html_as_doc(self._tmp_html_file, desktop), self._tmp_doc_file)
            # FileHelper.store_as_doc(Constant.Tmp_Doc_File, FileHelper.read_html_as_doc(Constant.Tmp_Html_File, desktop))
            # word_doc = AddLinksJob.open_as_doc(AddLinksJob._tmp_doc_file, desktop)

            SelectionHelper.change_term_color_document(Constant.Tmp_Html_File, desktop)
            word_doc = FileHelper.open_as_doc(Constant.Tmp_Html_File, desktop, False, False)

        except urllib2.HTTPError, e:
            LogManager.logger.exception(e)
            error_content = e.read()

            # if language problem
            if error_content.find('INPUT ISSUE'):

                msg_box = (ControlBuilder(self.ctx)
                           .msg_box()
                           .with_button_type(1)
                           .with_title('Error')
                           .with_label(ResourceManager.unsupported_language())
                           .with_msg_box_type('ERRORBOX')
                           .result())

            msg_box.execute()

        except Exception, e:
            LogManager.logger.exception(e)

    def show_p(self, dialog):
        dialog.setVisible(True)

    def show_progress(self):
        has_been_closed = True
        dialog_model = (ControlBuilder(self.ctx)
                        .dialog_model()
                        .with_width(120)
                        .with_height(30)
                        .result())

        dialog = (ControlBuilder(self.ctx)
                  .dialog()
                  .result())

        progress_label = (ControlBuilder(self.ctx)
                          .label()
                          .with_label(ResourceManager.working())
                          .result())

        toolkit = (ControlBuilder(self.ctx)
                   .toolkit()
                   .result())

        dialog.setModel(dialog_model)
        dialog_model.insertByName('progressLabel', progress_label)
        progress_label_control = dialog.getControl('progressLabel')
        progress_label_control.setPosSize(10, 10, 140, 30, POSSIZE)

        # dialog.setVisible(True)
        # dialog.createPeer(toolkit, None)

        return dialog

    @classmethod
    def store_as_doc(cls, document, path):
        args = (PropertyValue('FilteName', 0, 'MS Word 97', 0),)
        document.storeToURL(path, args)
        document.dispose()

    @classmethod
    def open_as_doc(cls, path, desktop):
        document = desktop.loadComponentFromURL("private:factory/swriter", "_blank", 0, ())
        text = document.getText()
        cursor = text.createTextCursor()
        cursor.gotoEnd(False)
        cursor.insertDocumentFromURL(path, ())

        selection = document.getCurrentController().getViewCursor()
        while True:
            old_pos = selection.getPosition()
            selection.goLeft(1, False)
            new_pos = selection.getPosition()

            if old_pos.X == new_pos.X and old_pos.Y == new_pos.Y:
                selection.goRight(1, True)
                selected_text = selection.getString()
                if selected_text == '\r\n':
                    selection.setString("")
                break

        return document


        # args = (PropertyValue('FilterName', 0, 'MS Word 97', 0),)
        # document = desktop.loadComponentFromURL(path, '_blank', 0, args)

        #return document

    @classmethod
    def read_html_as_doc(cls, path, desktop):
        document = desktop.loadComponentFromURL(path, '_blank', 0,
                                                (PropertyValue('Hidden', 0, 'True', 0),
                                                 PropertyValue('FilterName', 0, 'MS Word 97', 0),))

        return document

    @classmethod
    def read_file(cls, path):

        file_handle = open('C:\\Users\\oreshenski\\Source\\Python\\EuLinksChecker\\Extensions\\ec-tmp.html', 'r')
        file_content = file_handle.read();
        file_handle.close()

        return file_content

    @classmethod
    def write_file(cls, path, content):

            file_handle = open('C:\\Users\\oreshenski\\Source\\Python\\EuLinksChecker\\Extensions\\ec-tmp.html', 'w')
            file_handle.write(content)
            file_handle.close()


class RemoveSelectedLinkJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()

    def trigger(self, args):

        try:

            desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)

            document = desktop.getCurrentComponent()

            SelectionHelper.remove_hyperlink(document)

            #controller = document.getCurrentController()
            #selection = controller.getViewCursor().getString()

            #cursour =

        except Exception, e:
            LogManager.logger.exception(e)


class RemoveAllLinksJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()

    def trigger(self, args):

        desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)

        document = desktop.getCurrentComponent()

        text = document.getText()

        msg_box = (ControlBuilder(self.ctx)
                   .msg_box()
                   .with_button_type(2)
                   .with_title(Constant.Warnning)
                   .with_label(ResourceManager.confirm_remove_all_links())
                   .with_msg_box_type('WARNINGBOX')
                   .result())

        msg_result = msg_box.execute()

        if msg_result == 1:
            SelectionHelper.remove_all_hyperlink(text)


class RemoveLinksFromSelectionJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()
        LogManager.init()

    def trigger(self, args):
        try:

            desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)

            document = desktop.getCurrentComponent()

            selection = document.getCurrentController().getViewCursor()

            selected_text = selection.getText().createTextCursorByRange(selection)
            selected_string = selected_text.getString()

            if selected_string:
                removed_count = SelectionHelper.remove_all_hyperlink(selected_text)

                if removed_count == 0:
                    msg_box = (ControlBuilder(self.ctx)
                               .msg_box()
                               .with_button_type(1)
                               .with_title(Constant.Warnning)
                               .with_label(ResourceManager.mark_text_for_delete())
                               .with_msg_box_type('WARNINGBOX')
                               .result())

                    msg_box.execute()
            else:
                msg_box = (ControlBuilder(self.ctx)
                           .msg_box()
                           .with_button_type(1)
                           .with_title(Constant.Warnning)
                           .with_label(ResourceManager.mark_text_for_delete())
                           .with_msg_box_type('WARNINGBOX')
                           .result())

                msg_box.execute()

        except Exception, e:
            LogManager.logger.exception(e)


class InsertLinkIntoSelectionJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()

    def trigger(self, args):
        try:
            desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)
            document = desktop.getCurrentComponent()
            selection = document.getCurrentController().getViewCursor()
            selected_text = selection.getText().createTextCursorByRange(selection)
            selected_string = selected_text.getString()
            if selected_string:

                dialog_model = (ControlBuilder(self.ctx)
                                .dialog_model()
                                .with_width(120)
                                .with_height(60)
                                .with_title(Constant.Insert_Url)
                                .result())

                dialog = (ControlBuilder(self.ctx)
                          .dialog()
                          .result())

                toolkit = (ControlBuilder(self.ctx)
                           .toolkit()
                           .result())

                button = (ControlBuilder(self.ctx)
                          .button()
                          .with_label(Constant.Ok)
                          .result())

                label = (ControlBuilder(self.ctx)
                         .label()
                         .with_label(ResourceManager.input_url())
                         .result())

                input_ = (ControlBuilder(self.ctx)
                          .input()
                          .result())

                dialog.setModel(dialog_model)
                dialog_model.insertByName('input_url', input_)
                dialog_model.insertByName('btn_ok', button)
                dialog_model.insertByName('label', label)

                label_control = dialog.getControl('label')
                input_control = dialog.getControl('input_url')
                button_control = dialog.getControl('btn_ok')

                label_control.setPosSize(10, 10, 300, 25, POSSIZE)
                input_control.setPosSize(10, 30, 200, 25, POSSIZE)
                button_control.setPosSize(10, 65, 80, 20, POSSIZE)
                button_control.addActionListener(InsertLinkListener(self.ctx, dialog))

                dialog.setVisible(False)
                dialog.createPeer(toolkit, None)
                dialog.execute()

            else:
                msg_box = (ControlBuilder(self.ctx)
                           .msg_box()
                           .with_button_type(1)
                           .with_title(Constant.Warnning)
                           .with_label(ResourceManager.mark_text_for_insert())
                           .with_msg_box_type('WARNINGBOX')
                           .result())

                msg_box.execute()

        except Exception, e:
            LogManager.logger.exception(e)


class InsertLinkListener(unohelper.Base, XActionListener):
    def __init__(self, ctx, dialog):
        self.ctx = ctx
        self.dialog = dialog
        LogManager.init()

    def actionPerformed(self, event):
        try:
            input_ = self.dialog.getControl('input_url')
            url = input_.getText()
            self.dialog.endExecute()
            desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)
            document = desktop.getCurrentComponent()
            selection = document.getCurrentController().getViewCursor()
            range_ = selection.getText().createTextCursorByRange(selection)

            if url:
                range_.HyperLinkURL = url
        except Exception, e:
            LogManager.logger.exception(e)

    def disposing(self, source_event):
        pass


class CloseDialogListener(unohelper.Base, XActionListener):
    def __init__(self, dialog):
        self.dialog = dialog
        LogManager.init()

    def actionPerformed(self, event):
        try:
            self.dialog.endExecute()
        except Exception, e:
            LogManager.logger.exception(e)


class ShowRelevantNationalCaseRefJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()

    def trigger(self, args):
        Helper.webbrowser_navigate_to(self.ctx, Constant.EurocasesRefType_NationalCaseType)


class ShowRelevantNationalLegislationRefJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()

    def trigger(self, args):
        Helper.webbrowser_navigate_to(self.ctx, Constant.EurocasesRefType_NationalLegislationType)


class ShowRelevantEuCaseRefJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()

    def trigger(self, args):
        Helper.webbrowser_navigate_to(self.ctx, Constant.EurocasesRefType_EuCaseType)


class ShowRelevantEuLegislationRefJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()

    def trigger(self, args):
        Helper.webbrowser_navigate_to(self.ctx, Constant.EurocasesRefType_EuLegislationType)


class ShowAllRelevantRefJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()

    def trigger(self, args):
        Helper.webbrowser_navigate_to(self.ctx, Constant.EurocasesRefType_AllType)


class ShowHintJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        LogManager.init()

    def trigger(self, args):
        try:
            desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)
            document = desktop.getCurrentComponent()
            url, color = SelectionHelper.get_url_and_color(document)
            if color == 32768 and url:
                service_url = HintUrlBuilder.build_url(url)
                webbrowser.open_new(service_url)

        except Exception, e:
            LogManager.logger.exception(e)


class ShowRelevantNationalCaseTermJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx

    def trigger(self, args):
        Helper.webbrowser_term_navigate_to(self.ctx, Constant.EurocasesRefType_NationalLegislationType)


class ShowRelevantNationalLegislationTermJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx

    def trigger(self, args):
        Helper.webbrowser_term_navigate_to(self.ctx, Constant.EurocasesRefType_NationalLegislationType)


class ShowRelevantEuCaseLawTermJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx

    def trigger(self, args):
        Helper.webbrowser_term_navigate_to(self.ctx, Constant.EurocasesRefType_EuCaseType)


class ShowRelevantEuLegislationTermJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx

    def trigger(self, args):
        Helper.webbrowser_term_navigate_to(self.ctx, Constant.EurocasesRefType_EuLegislationType)


class ShowAllRelevantTermJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx

    def trigger(self, args):
        Helper.webbrowser_term_navigate_to(self.ctx, Constant.EurocasesRefType_AllType)


class ExportToXmlJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        self.linking_service = LinkingService('xx')
        LogManager.init()

    def trigger(self, args):
        try:
            desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)
            document = desktop.getCurrentComponent()

            FileHelper.store_as_html(Constant.Tmp_Html_File, document)

            html = FileHelper.read_file(Constant.Tmp_Html_File_Read)

            xml = self.linking_service.export_xml(html)

            file_picker = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.ui.dialogs.FilePicker",
                                                                            self.ctx)
            file_picker.setTitle('Save as')
            file_picker.appendFilter("XML", "*.xml")
            file_picker.initialize((10,),)

            ok = file_picker.execute()
            if ok == 1:
                files = file_picker.getFiles()
                if files.__len__() > 0:
                    path = files[0]

                    # file:///C:/Users/oreshenski/Desktop/000000000.xml
                    path = path.replace('file:///', '')
                    path = path.replace('/', '\\')

                    FileHelper.write_file(path, xml)
        except Exception, e:
            try:
                msg_box = (ControlBuilder(self.ctx)
                           .msg_box()
                           .with_button_type(1)
                           .with_title('Error')
                           .with_label(ResourceManager.error())
                           .with_msg_box_type('ERRORBOX')
                           .result())
                msg_box.execute()
            except Exception, e:
                LogManager.logger.exception(e)

            LogManager.logger.exception(e)


class GetLongCitateJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        self.linking_service = LinkingService('xx')
        LogManager.init()

    def trigger(self, args):
        desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)
        document = desktop.getCurrentComponent()
        url, color = SelectionHelper.get_url_and_color(document)

        try:
            if url:

                celex_index = url.find('CELEX:')
                if celex_index >= 0:
                    provision = url[celex_index + 'CELEX:'.__len__():]
                    doc_number = provision.split('#')[0]

                citate = self.linking_service.get_citate(doc_number=doc_number,
                                                         lang_id=4,
                                                         citate_type=Constant.FullCitate)
                if citate:
                    dialog_model = (ControlBuilder(self.ctx)
                                    .dialog_model()
                                    .with_width(200)
                                    .with_height(110)
                                    .with_title(ResourceManager.citation())
                                    .result())

                    dialog = (ControlBuilder(self.ctx)
                              .dialog()
                              .result())

                    toolkit = (ControlBuilder(self.ctx)
                               .toolkit()
                               .result())

                    button = (ControlBuilder(self.ctx)
                              .button()
                              .with_label(Constant.Ok)
                              .result())

                    input_ = (ControlBuilder(self.ctx)
                              .input()
                              .result())

                    input_.ReadOnly = True
                    input_.MultiLine = True

                    dialog.setModel(dialog_model)
                    dialog_model.insertByName('input_url', input_)
                    dialog_model.insertByName('btn_ok', button)

                    input_control = dialog.getControl('input_url')
                    button_control = dialog.getControl('btn_ok')

                    input_control.Text = citate
                    input_control.setPosSize(10, 10, 390, 165, POSSIZE)
                    button_control.setPosSize(10, 175, 80, 20, POSSIZE)
                    button_control.addActionListener(CloseDialogListener(dialog))

                    dialog.setVisible(False)
                    dialog.createPeer(toolkit, None)
                    dialog.execute()

        except Exception, e:
            LogManager.logger.exception(e)


        # /api/Doc/Cite/{langId}/{docNumber}/{citeType}
        # citeType - int: ShortCite = 1, LongCite = 2
        # return:
        # DocType: 1 - caselaw, 2 - legislation
        # Text - string
        # http://web.eucases.eu:8080/api/Doc/Cite/{langId}/{docNumber}/{citeType}


class GetShortCitateJob(unohelper.Base, XJobExecutor):
    def __init__(self, ctx):
        self.ctx = ctx
        self.linking_service = LinkingService('xx')
        LogManager.init()

    def trigger(self, args):
        desktop = self.ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", self.ctx)
        document = desktop.getCurrentComponent()
        url, color = SelectionHelper.get_url_and_color(document)

        try:
            if url:

                celex_index = url.find('CELEX:')
                if celex_index >= 0:
                    provision = url[celex_index + 'CELEX:'.__len__():]
                    doc_number = provision.split('#')[0]

                citate = self.linking_service.get_citate(doc_number=doc_number,
                                                         lang_id=4,
                                                         citate_type=Constant.ShortCitate)
                if citate:
                    dialog_model = (ControlBuilder(self.ctx)
                                    .dialog_model()
                                    .with_width(200)
                                    .with_height(110)
                                    .with_title(ResourceManager.citation())
                                    .result())

                    dialog = (ControlBuilder(self.ctx)
                              .dialog()
                              .result())

                    toolkit = (ControlBuilder(self.ctx)
                               .toolkit()
                               .result())

                    button = (ControlBuilder(self.ctx)
                              .button()
                              .with_label(Constant.Ok)
                              .result())

                    input_ = (ControlBuilder(self.ctx)
                              .input()
                              .result())

                    input_.ReadOnly = True
                    input_.MultiLine = True

                    dialog.setModel(dialog_model)
                    dialog_model.insertByName('input_url', input_)
                    dialog_model.insertByName('btn_ok', button)

                    input_control = dialog.getControl('input_url')
                    button_control = dialog.getControl('btn_ok')

                    input_control.Text = citate
                    input_control.setPosSize(10, 10, 390, 165, POSSIZE)
                    button_control.setPosSize(10, 175, 80, 20, POSSIZE)
                    button_control.addActionListener(CloseDialogListener(dialog))

                    dialog.setVisible(False)
                    dialog.createPeer(toolkit, None)
                    dialog.execute()

        except Exception, e:
            LogManager.logger.exception(e)


        # /api/Doc/Cite/{langId}/{docNumber}/{citeType}
        # citeType - int: ShortCite = 1, LongCite = 2
        # return:
        # DocType: 1 - caselaw, 2 - legislation
        # Text - string
        # http://web.eucases.eu:8080/api/Doc/Cite/{langId}/{docNumber}/{citeType}


class JobExecutor (XJob, unohelper.Base):
    def __init__ (self, ctx):
        self.ctx = ctx

    def execute (self, args):
        if not args:
            return
        # what version of the software?
        xSettings = getConfigSetting("org.openoffice.Setup/Product", False)
        sProdName = xSettings.getByName("ooName")
        sVersion = xSettings.getByName("ooSetupVersion")
        if (sProdName == "LibreOffice" and sVersion < "3.4") or sProdName == "OpenOffice.org":
            return

        # what event?
        bCorrectEvent = False
        for arg in args:
            if arg.Name == "Environment":
                for v in arg.Value:
                    if v.Name == "EnvType" and v.Value == "DOCUMENTEVENT":
                        bCorrectEvent = True
                    elif v.Name == "EventName":
                        pass
                        # check is correct event
                        #print "Event: %s" % v.Value
                    elif v.Name == "Model":
                        model = v.Value
        if bCorrectEvent:
            if model.supportsService("com.sun.star.text.TextDocument"):
                xController = model.getCurrentController()
                if xController:
                    xController.registerContextMenuInterceptor(ContextMenuInterceptor(self.ctx))


class ReferenceUrlBuilder:
    def __init__(self):
        pass

    @staticmethod
    def build_url(url_type, url):
        if url:
            url = url.replace('#', '/')
            # http://demo.eurocases.eu/api/Doc/DocInLinks/NatCL/4/1/100/32015D1566/
            # http://demo.eurocases.eu/api/Doc/DocInLinks/NatCL/4/1/100/11997M/Art19

            # http://demo.eurocases.eu/api/Doc/DocInLinks/NatL/4/1/100/32015D1566/
            # http://demo.eurocases.eu/api/Doc/DocInLinks/EuCL/4/1/100/32015D1566/
            # http://demo.eurocases.eu/api/Doc/DocInLinks/EuL/4/1/100/32015D1566/
            # http://demo.eurocases.eu/api/Doc/DocInLinks/All/4/1/100/32015D1566/

            celexsStart = url.find('CELEX:')
            if celexsStart > -1:
                celexAndProvision = url[celexsStart + 'CELEX:'.__len__():]
                celex = celexAndProvision
                provision = ''

                url = Constant.EurocasesRef_BaseUrl + url_type + '/4/1/100/' + celex

                return  url
        else:
            return ''


class HintUrlBuilder:
    def __init__(self):
        pass

    @staticmethod
    def build_url(url):
        if url:
            url = url.replace('#', '/')

            celexStart = url.find('CELEX:')
            if celexStart > -1:
                url = Constant.EurocasesHint_BaseUrl + url[celexStart + 'CELEX:'.__len__():]

                return url
        else:
            return ''

        # http://demo.eurocases.eu/api/Doc/ParHint/4/4/32015D1566
        # http://demo.eurocases.eu/api/Doc/ParHint/4/4/11997M/Art19

        pass


class TermsUrlBuilder:
    def __init__(self):
        pass

    @staticmethod
    def build_url(url_type, url):
        if url:
            # http://demo.eurocases.eu/api/Doc/SearchByXmlId/1172/NatCL/4/4
            # http://demo.eurocases.eu/api/Doc/SearchByXmlId/1172/all/4/4
            url = url.replace('all', url_type)
            return url

        return ''


class SelectionHelper:
    def __init__(self):
        pass

    @staticmethod
    def get_url_and_color(document):

        controller = document.getCurrentController()
        text = document.getText()
        selection = controller.getViewCursor()

        iterations = 0
        iterations_ = 0
        while True:
            # textCursor = text.createTextCursorByRange(selection)
            textCursor = selection.getStart()

            oldPos = selection.getPosition()
            oldUrl = textCursor.HyperLinkURL
            oldColor = textCursor.CharColor

            selection.goLeft(1, True)

            newPos = selection.getPosition()
            # textCursor = text.createTextCursorByRange(selection)
            textCursor = selection.getStart()
            newUrl = textCursor.HyperLinkURL
            newColor = textCursor.CharColor

            is_color_ok = newColor == Constant.Green_Color or newColor == Constant.Red_Color
            is_url_ok = newUrl != ''

            if not is_color_ok or not is_url_ok:
                txt = selection.getString()
                if newColor == -1 and (txt.startswith(' ') or txt.startswith('\r\n')):
                    selection.goRight(1, True)
                break

            has_moved = oldPos.X != newPos.X or oldPos.Y != newPos.Y
            has_url_changed = oldUrl != newUrl
            are_colors_diff = oldColor != newColor
            if not has_moved or has_url_changed:
                if has_url_changed and are_colors_diff and newColor == -1:
                    selection.goRight(1, True)
                # iterations += 1
                break
            iterations += 1

        selection.goRight(iterations, False)
        selection.goLeft(iterations, False)
        selection.goRight(iterations, True)

        while True:
            #textCursor = text.createTextCursorByRange(selection)
            textCursor = selection.getEnd()

            oldPos = selection.getPosition()
            oldUrl = textCursor.HyperLinkURL
            oldColor = textCursor.CharColor

            selection.goRight(1, True)

            newPos = selection.getPosition()
            # textCursor = text.createTextCursorByRange(selection)
            textCursor = selection.getEnd()
            newUrl = textCursor.HyperLinkURL
            newColor = textCursor.CharColor

            is_color_ok = newColor == Constant.Green_Color or newColor == Constant.Red_Color
            is_url_ok = newUrl != ''

            if not is_color_ok or not is_url_ok:
                txt = selection.getString()
                if txt.endswith(' ') or txt.endswith(','):
                    selection.goLeft(1, True)
                break

            has_moved = oldPos.X != newPos.X or oldPos.Y != newPos.Y
            has_url_changed = oldUrl != newUrl
            are_colors_diff = oldColor != newColor
            if not has_moved or has_url_changed:
                if has_url_changed and are_colors_diff and newColor == -1:
                    selection.goLeft(1, True)
                break
            iterations_ += 1

        #finalSelection = selection.getStart()

        hyperlink = selection.HyperLinkURL
        color = selection.CharColor

        # LogManager.logger.info('#' + hyperlink)
        # LogManager.logger.info('#' + str(color))

        selection.goLeft(iterations_ + iterations, True)
        selection.goRight(iterations, False)

        return hyperlink, color

    @staticmethod
    def remove_hyperlink(document):
        controller = document.getCurrentController()
        text = document.getText()
        selection = controller.getViewCursor()

        iterations = 0
        iterations_ = 0
        while True:
            # textCursor = text.createTextCursorByRange(selection)
            textCursor = selection.getStart()

            oldPos = selection.getPosition()
            oldUrl = textCursor.HyperLinkURL
            oldColor = textCursor.CharColor

            selection.goLeft(1, True)

            newPos = selection.getPosition()
            # textCursor = text.createTextCursorByRange(selection)
            textCursor = selection.getStart()
            newUrl = textCursor.HyperLinkURL
            newColor = textCursor.CharColor

            is_color_ok = newColor == Constant.Green_Color or newColor == Constant.Red_Color
            is_url_ok = newUrl != ''

            if not is_color_ok or not is_url_ok:
                txt = selection.getString()
                if newColor == -1 and (txt.startswith(' ') or txt.startswith('\r\n')):
                    selection.goRight(1, True)
                break

            has_moved = oldPos.X != newPos.X or oldPos.Y != newPos.Y
            has_url_changed = oldUrl != newUrl
            are_colors_diff = oldColor != newColor
            if not has_moved or has_url_changed:
                if has_url_changed and are_colors_diff and newColor == -1:
                    selection.goRight(1, True)
                break
            iterations += 1

        selection.goRight(iterations, False)
        selection.goLeft(iterations, False)
        selection.goRight(iterations, True)

        while True:
            #textCursor = text.createTextCursorByRange(selection)
            textCursor = selection.getEnd()

            oldPos = selection.getPosition()
            oldUrl = textCursor.HyperLinkURL
            oldColor = textCursor.CharColor

            selection.goRight(1, True)

            newPos = selection.getPosition()
            # textCursor = text.createTextCursorByRange(selection)
            textCursor = selection.getEnd()
            newUrl = textCursor.HyperLinkURL
            newColor = textCursor.CharColor

            is_color_ok = newColor == Constant.Green_Color or newColor == Constant.Red_Color
            is_url_ok = newUrl != ''

            if not is_color_ok or not is_url_ok:
                txt = selection.getString()
                if txt.endswith(' ') or txt.endswith(','):
                    selection.goLeft(1, True)
                break

            has_moved = oldPos.X != newPos.X or oldPos.Y != newPos.Y
            has_url_changed = oldUrl != newUrl
            are_colors_diff = oldColor != newColor
            if not has_moved or has_url_changed:
                if has_url_changed and are_colors_diff and newColor == -1:
                    selection.goLeft(1, True)
                break
            iterations_ += 1

        color = selection.CharColor

        if color == Constant.Green_Color or color == Constant.Red_Color:
            selection.HyperLinkURL = ''
            selection.CharColor = -1
            selection.CharUnderline = 0
            # if selection.ParaStyleName:
                # selection.ParaStyleName = ''

        selection.goLeft(iterations_ + iterations, True)
        selection.goRight(iterations, False)

    @staticmethod
    def remove_all_hyperlink(current):
        removed = 0
        try:
            is_enumerable = hasattr(current, 'createEnumeration')
            if is_enumerable:
                current_enumerator = current.createEnumeration()
                while current_enumerator.hasMoreElements():
                    inner = current_enumerator.nextElement()
                    removed += SelectionHelper.remove_all_hyperlink(inner)
            else:
                is_table = hasattr(current, 'getCellNames')
                if is_table:
                    cell_names = current.getCellNames()
                    for cell_name in cell_names:
                        cell = current.getCellByName(cell_name)
                        removed += SelectionHelper.remove_all_hyperlink(cell)
                else:
                    color = current.CharColor
                    if color and (color == Constant.Green_Color or color == Constant.Red_Color):
                        current.CharColor = -1
                        current.HyperLinkURL = ''
                        current.CharUnderline = 0
                        removed += 1
        except Exception, e:
            LogManager.logger.exception(e)
        return removed

    @staticmethod
    def change_term_color(current):
        try:
            is_enumerable = hasattr(current, 'createEnumeration')
            if is_enumerable:
                current_enumerator = current.createEnumeration()
                while current_enumerator.hasMoreElements():
                    inner = current_enumerator.nextElement()
                    SelectionHelper.change_term_color(inner)
            else:
                is_table = hasattr(current, 'getCellNames')
                if is_table:
                    cell_names = current.getCellNames()
                    for cell_name in cell_names:
                        cell = current.getCellByName(cell_name)
                        SelectionHelper.change_term_color(cell)
                else:
                    color = current.CharColor
                    if color and (color == Constant._Red_Color):
                        current.CharColor = Constant.Red_Color
        except Exception, e:
            LogManager.logger.exception(e)

    @staticmethod
    def change_term_color_document(file_path, desktop):
        try:
            document = FileHelper.open_as_doc(file_path, desktop, True, True)
            current = document.getText()

            is_enumerable = hasattr(current, 'createEnumeration')
            if is_enumerable:
                current_enumerator = current.createEnumeration()
                while current_enumerator.hasMoreElements():
                    inner = current_enumerator.nextElement()
                    SelectionHelper.change_term_color(inner)
            else:
                is_table = hasattr(current, 'getCellNames')
                if is_table:
                    cell_names = current.getCellNames()
                    for cell_name in cell_names:
                        cell = current.getCellByName(cell_name)
                        SelectionHelper.change_term_color(cell)
                else:
                    color = current.CharColor
                    if color and (color == Constant._Red_Color):
                        current.CharColor = Constant.Red_Color

            # file_path = file_path.replace('file:///', '')
            # file_path = file_path.replace('/', '\\')
            args = (PropertyValue('FilterName', 0, 'HTML (StarWriter)', 0),)
            document.storeToURL(file_path, args)
            document.dispose()
        except Exception, e:
            LogManager.logger.exception(e)

    # SelectionHelper.change_term_color(text)


class LinkingService:
    """ defines methods for communication with the rest linking api """

    _LINKS_CONTROLLER = 'Links'
    _PUT_HTML_LINKS_ACTION = 'PutHtmlLinksWordAddin'

    _restApiUrl = ''
    _restApiUrlBuilder = None

    def __init__(self, rest_api_url):

        Guard.assure_not_empty(rest_api_url, 'rest api base url')

        self._restApiUrl = rest_api_url
        self._restApiUrlBuilder = RestApiUrlBuilder(rest_api_url)

    def put_links_for_html(self, html):
        """ returns the html with inserted links and terms """

        request_url = (
            self._restApiUrlBuilder
            .with_controller(self._LINKS_CONTROLLER)
            .with_action(self._PUT_HTML_LINKS_ACTION)
            .result()
        )

        request = urllib2.Request(request_url, html)
        request.add_header('Content-Type', 'text/html;charset=windows-1251') # req.add_header('Content-Type', 'text/html;charset=utf-8')
        response = urllib2.urlopen(request)
        content = response.read()

        encoding = response.headers.getparam('charset') #.split('charset=')[-1]
        content = content.decode('utf8').encode('windows-1251')

        # LogManager.logger.info(encoding)
        # LogManager.logger.info(content)

        #request_headers = {'content-type': 'text/plain'}

        #response = requests.post(request_url, data=html, headers=request_headers)

        #if response.status_code == 200:
            #decoded = HTMLParser().unescape(response.text)
            #return decoded

        return content

    def export_xml(self, html):

        request_url = 'http://techno.eucases.eu/FrontEndREST/api/Links/GenerateXml/'

        request = urllib2.Request(request_url, html)
        request.add_header('Content-Type', 'text/html;charset=windows-1251')
        response = urllib2.urlopen(request)
        content = response.read()

        content = content.decode('utf8').encode('windows-1251')

        return content

    def get_citate(self, doc_number, lang_id, citate_type):
        request_url = 'http://web.eucases.eu:8080/api/Doc/Cite/' + str(lang_id) + '/' + doc_number + '/' + str(citate_type)

        response = urllib2.urlopen(request_url).read()
        json_data = json.loads(response)
        return json_data['Text']


class RestApiUrlBuilder:
    """ provides basic functionality for create rest api urls """

    """ represents the base address of the rest api """
    _base_address = ''

    """ container for the result of the builder """
    _endpoint = ''

    _has_any_parameter = False

    def __init__(self, rest_api_base_address):

        Guard.assure_not_empty(rest_api_base_address, 'res_api_base_address')

        self._base_address = rest_api_base_address
        self._base_address = Helper.append_back_slash(self._base_address)

        self._endpoint = self._base_address

    def with_controller(self, controller):
            """ appends the controller to the result rest api url """

            Guard.assure_not_empty(controller, 'controller')

            self._endpoint = Helper.concat_with_back_slash(self._endpoint, controller)

            return self

    def with_action(self, action):
        """ appends the action to the result rest api url """

        Guard.assure_not_empty(action, 'action')

        self._endpoint = Helper.concat_with_back_slash(self._endpoint, action)

        return self

    def with_parameter(self, name, value):
        """ appends the parameter to the result rest api url """

        Guard.assure_not_empty(name, 'name')
        Guard.assure_not_empty(value, 'value')

        append_charachter = '?'
        if self._has_any_parameter:
            append_charachter = '&'

        self._endpoint = Helper.append_if_not_ends_with(self._endpoint, append_charachter)
        self._endpoint = Helper.append_if_not_ends_with(self._endpoint, name)
        self._endpoint = Helper.append_if_not_ends_with(self._endpoint, '=')
        self._endpoint = Helper.append_if_not_ends_with(self._endpoint, value)

        return self

    def result(self):
        """ returns the builded rest api url and
             clears the current state for further buildings """

        end_point = self._endpoint
        self._endpoint = ''

        return end_point


class MenuItemBuilder:
    def __init__(self, contextMenu):
        self.contextMenu = contextMenu
        self.text = ''
        self.commandURL = ''

    def with_text(self, text):
        self.text = text

        return self

    def with_command(self, commandURL):
        self.commandURL = commandURL

        return self

    def result(self):
        menuItem = self.contextMenu.createInstance("com.sun.star.ui.ActionTrigger")

        menuItem.setPropertyValue('Text', self.text)
        if self.commandURL:
            menuItem.setPropertyValue('CommandURL', self.commandURL)

        self.text = ''
        self.commandURL = ''

        return menuItem


class ControlBuilder:
    def __init__(self, ctx):
        self._type = ''
        self._x = 0
        self._y = 0
        self._width = 0
        self._height = 0
        self._title = 0
        self._ctx = ctx
        self._is_visible = None
        self._label = ''
        self._name = ''
        self._is_msg_box = False
        self._btn_type = 0
        self._msg_box_type = 'WARNINGBOX'
        self._title = ''

    def dialog_model(self):
        self._type = Constant.ControlService_DialogModel
        return self

    def dialog(self):
        self._type = Constant.ControlService_Dialog
        return self

    def toolkit(self):
        self._type = Constant.ControlService_Toolkit
        return self

    def button(self):
        self._type = Constant.ControlService_ButtonModel
        return self

    def label(self):
        self._type = Constant.ControlService_LabelModel
        return self

    def msg_box(self):
        self._type = Constant.ControlService_Toolkit
        self._is_msg_box = True
        return self

    def input(self):
        self._type = Constant.ControlService_Input
        return  self

    def progress_bar(self):
        self._type = 'com.sun.star.awt.UnoControlProgressBarModel'
        return self

    def with_position_x(self, x):
        self._x = x
        return self

    def with_position_y(self, y):
        self._y = y
        return self

    def with_width(self, width):
        self._width = width
        return self

    def with_height(self, height):
        self._height = height
        return self

    def with_title(self, title):
        self._title = title
        return self

    def is_visible(self, is_visible):
        self._is_visible = is_visible
        return self

    def with_label(self, label):
        self._label = label
        return self

    def with_name(self, name):
        self._name = name
        return self

    def with_button_type(self, btn_type):
        self._btn_type = btn_type
        return self

    def with_msg_box_type(self, msg_box_type):
        self._msg_box_type = msg_box_type
        return self

    def with_title(self, title):
        self._title = title
        return self

    def result(self):
        obj = self._ctx.ServiceManager.createInstanceWithContext(self._type, self._ctx)
        if self._is_msg_box:
            msg_box = obj.createMessageBox(None, self._msg_box_type, self._btn_type, self._title, self._label)
            return msg_box
        else:
            if self._x and hasattr(obj, 'PositionX'):
                obj.PositionX = self._x
            if self._y and hasattr(obj, 'PositionY'):
                obj.PositionY = self._y
            if self._width and hasattr(obj, 'Width'):
                obj.Width = self._width
            if self._height and hasattr(obj, 'Height'):
                obj.Height = self._height
            if self._title and hasattr(obj, 'Title'):
                obj.Title = self._title
            if self._is_visible and hasattr(obj, 'setVisible'):
                obj.setVisible(self._is_visible)
            if self._label and hasattr(obj, 'Label'):
                obj.Label = self._label
            if self._name and hasattr(obj, 'Name'):
                obj.Name = self._name
            if self._btn_type and hasattr(obj, 'PushButtonType'):
                obj.PushButtonType = self._btn_type

        return obj


class LogManager:
    logger = None

    @staticmethod
    def init():
        logging.basicConfig(level=logging.DEBUG,
                            format='%(asctime)s %(levelname)s %(message)s',
                            filename='C:\\Windows\\Temp\\eulinkscheckerlog.log',
                            filemode='w')

        logger = logging.getLogger('eulinkschecker')

        LogManager.logger = logger

    def __init__(self):
        pass


class Guard:
    @staticmethod
    def assure_not_empty(string, param_name):
        """ Raises exception if the string is empty """

        if not string:
            raise Exception(param_name + ' can not be empty')


class Helper:
    @staticmethod
    def append_if_not_ends_with(string, ends_with):
        """ appends the endsWith string if the input does
             not already ends with it """

        if not string.endswith(ends_with):
            string = string + ends_with

        return string

    @staticmethod
    def append_back_slash(string):
        """ append backslash at the end
         of the input if the input does not ends with backslash """

        return Helper.append_if_not_ends_with(string, '/')

    @staticmethod
    def concat_with_back_slash(parametera, parameterb):
        """ concatenate parameterA and parameterB with backslash in betweeen """

        if not parametera.endswith('/'):
            return parametera + '/' + parameterb

        return parametera + parameterb

    @staticmethod
    def webbrowser_navigate_to(ctx, refType):
        try:

            desktop = ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", ctx)
            document = desktop.getCurrentComponent()
            url, color = SelectionHelper.get_url_and_color(document)
            if color == Constant.Green_Color and url:
                eurocasesServiceUrl = ReferenceUrlBuilder.build_url(refType, url)
                webbrowser.open_new(eurocasesServiceUrl)

        except Exception, e:
            LogManager.logger.exception(e)

    @staticmethod
    def webbrowser_term_navigate_to(ctx, ref_type):
        try:
            desktop = ctx.ServiceManager.createInstanceWithContext("com.sun.star.frame.Desktop", ctx)
            document = desktop.getCurrentComponent()
            url, color = SelectionHelper.get_url_and_color(document)
            if color == Constant.Red_Color and url:
                eurocasesServiceUrl = TermsUrlBuilder.build_url(ref_type, url)
                webbrowser.open_new(eurocasesServiceUrl)
        except Exception, e:
            LogManager.logger.exception(e)


class FileHelper:
    def __init__(self):
        pass

    @staticmethod
    def store_as_html(path, document):
        args = (PropertyValue('FilterName', 0, 'HTML (StarWriter)', 0),)
        document.storeToURL(path, args)

    @staticmethod
    def read_file(path):
        file_handle = open(path, 'r')
        file_content = file_handle.read()
        file_handle.close()

        return file_content

    @staticmethod
    def write_file(path, content):
        file_handle = open(path, 'w')
        # file_handle = open('C:\\Users\\oreshenski\\Source\\Python\\EuLinksChecker\\Extensions\\ec-tmp.xml', 'w')
        file_handle.write(content)
        file_handle.close()

    @staticmethod
    def store_as_html(path, document):
        args = (PropertyValue('FilterName', 0, 'HTML (StarWriter)', 0),)
        document.storeToURL(path, args)

    @staticmethod
    def store_as_doc(path, document):
        args = (PropertyValue('FilterName', 0, 'MS Word 97', 0),)
        document.storeToURL(path, args)
        document.dispose()

    @classmethod
    def read_html_as_doc(cls, path, desktop):
        document = desktop.loadComponentFromURL(path, '_blank', 0,
                                                (PropertyValue('Hidden', 0, 'True', 0),
                                                 PropertyValue('FilterName', 0, 'MS Word 97', 0),))
        return document

    @staticmethod
    def open_as_doc(path, desktop, to_clean, to_hide):

        args = ()
        if to_hide:
            args = (PropertyValue('Hidden', 0, True, 0),)
        document = desktop.loadComponentFromURL("private:factory/swriter", "_blank", 0, args)
        text = document.getText()

        cursor = text.createTextCursor()
        # cursor.gotoEnd(False)
        cursor.insertDocumentFromURL(path, ())

        if to_clean:
            selection = document.getCurrentController().getViewCursor()
            while True:
                old_pos = selection.getPosition()
                selection.goLeft(500, False)
                new_pos = selection.getPosition()

                if old_pos.X == new_pos.X and old_pos.Y == new_pos.Y:
                    selection.goRight(1, True)
                    selected_text = selection.getString()
                    if selected_text == '\r\n':
                        selection.setString("")
                    else:
                        selection.collapseToStart()
                    break

        return document


class ResourceManager:

    bg_map = \
        {
            # cAll
            "All": "Всички",

            # cCancel
            "Cancel": "Откажи",

            # cCannotAddLink
            "MarkTextForInsert": "Моля, маркирайте текст, за да вмъкнете връзка.",

            # cConfirmation
            "Confirm": "Потвърждение",

            # cErrorOccured
            "ErrorOccured": "Възникна грешка.",

            # cInsertLink
            "InsertLink": "Постави връзка",

            # cm_AllDocuments
            "AllDocuments": "Всички документи",

            # cm_DocumentsIndexedWithThisTerm
            "IndexedWithTerm": "Документи, индексирани с този термин:",

            # cm_DocumentsReferringToThisAct
            "ReferingWithProvision": "Документи, препращащи към този акт / разпоредба:",

            # cm_EuCL
            "EuLaw": "Съдебна практика на ЕС",

            # cm_EuL
            "EuLegislation": "Законодателство на ЕС",

            # cm_NaCL
            "NationalLaw": "Национална съдебна практика",

            # cm_NaL
            "NationalLegislation": "Национално законодателство",

            # cm_RemoveLink
            "RemoveLink": "Изтрий връзката",

            # cm_ShowToolTip
            "ShowToolTip": "Покажи подсказка",

            # cMsgNoLinksSetByELCFound
            "NoLinksFound": "Не са намерени връзки, поставени от EULinksChecker",

            # cOK
            "OK": "Потвърди",

            # cPlsSelLink
            "MarkAutomatedLink": "Моля, маркирайте текст с връзки, поставени от EULinksChecker",

            # cPlzConfirm
            "ConfirmLinking": "EULinksChecker ще провери дали съществуват препратки към "
                              "законодателството и съдебната практика на Европейския съюз и ще постави връзки към цитираните нормативни актове. "
                              "Това може да отнеме известно време. Моля, потвърдете!",

            # cPlzConfirmRmLnk
            "ConfirmRemoveAutoLink":"Моля, потвърдете изтриването на връзките, поставени от EULinksChecker.",

            "LongCitate": "Show long citation",

            "Citation": "Citation",

            "ShortCitate": "Show short citation",

            "CheckingForLinks": "Проверяване за връзки..."
        }

    en_map = \
        {
            # cAll
            "All": "All documents",

            # cCancel
            "Cancel": "Cancel",

            # cCannotAddLink
            "MarkTextForInsert": "Please, select text to insert a link.",

            # cConfirmation
            "Confirm": "Confirmation",

            # cErrorOccured
            "ErrorOccured": "Error occured.",

            # cInsertLink
            "InsertLink": "Insert link",

            # cm_AllDocuments
            "AllDocuments": "All documents",

            # cm_DocumentsIndexedWithThisTerm
            "IndexedWithTerm": "Documents indexed with this term:",

            # cm_DocumentsReferringToThisAct
            "ReferingWithProvision": "Documents referring to this act / provision:",

            # cm_EuCL
            "EuLaw": "EU case law",

            # cm_EuL
            "EuLegislation": "EU legislation",

            # cm_NaCL
            "NationalLaw": "National case law",

            # cm_NaL
            "NationalLegislation": "National legislation",

            # cm_RemoveLink
            "RemoveLink": "Remove link",

            # cm_ShowToolTip
            "ShowToolTip": "Show tooltip",

            # cMsgNoLinksSetByELCFound
            "NoLinksFound": "No links set by EULinksChecker have been found.",

            # cOK
            "OK": "OK",

            # cPlsSelLink
            "MarkAutomatedLink": "Please select text with links set by EULinksChecker.",

            # cPlzConfirm
            "ConfirmLinking": "EULinksChecker will check for existence of references to "
                              "EU legislation and case law and will set links to the cited legal acts."
                              " This may take a while. Please, confirm!",

            # cPlzConfirmRmLnk
            "ConfirmRemoveAutoLink": "Please confirm the removal of links set by EULinksChecker.",

            "EnterUrl": "Please enter the complete URL-address.",

            "LongCitate": "Show long citation",

            "Citation": "Citation",

            "ShortCitate": "Show short citation",

            "CheckingForLinks": "Checking for links...",

            "UnsupportedLang": "Linking failed. "
                                    "Unsupported langauge of the text has been detected. "
                                    "Currently supported languages are: English, French, Deutch, Bulgarian"
        }

    fr_map = \
        {
            # cAll
            "All": "Tous",

            # cCancel
            "Cancel": "Annuler",

            # cCannotAddLink
            "MarkTextForInsert": "Veuillez s?lectionner le texte pour ins?rer un lien.",

            # cConfirmation
            "Confirm": "Confirmation",

            # cErrorOccured
            "ErrorOccured": "Еrreur est survenue.",

            # cInsertLink
            "InsertLink": "Ins?rer le lien",

            # cm_AllDocuments
            "AllDocuments": "Tous les documents",

            # cm_DocumentsIndexedWithThisTerm
            "IndexedWithTerm": "Documents index?s avec ce terme:",

            # cm_DocumentsReferringToThisAct
            "ReferingWithProvision": "Documents faisant r?f?rence ? cet acte / disposition:",

            # cm_EuCL
            "EuLaw": "Jurisprudence de l'UE",

            # cm_EuL
            "EuLegislation": "L?gislation de l'UE",

            # cm_NaCL
            "NationalLaw": "Jurisprudence nationale",

            # cm_NaL
            "NationalLegislation": "L?gislation nationale",

            # cm_RemoveLink
            "RemoveLink": "Supprimer le lien",

            # cm_ShowToolTip
            "ShowToolTip": "Afficher info-bulle",

            # cMsgNoLinksSetByELCFound
            "NoLinksFound": "Aucun lien ?tabli par EULinksChecker.",

            # cOK
            "OK": "OK",

            # cPlsSelLink
            "MarkAutomatedLink": "Veuillez s?lectionner le texte avec des liens ?tablis par EULinksChecker.",

            # cPlzConfirm
            "ConfirmLinking": "EULinksChecker v?rifiera s’il existe des r?f?rences menant vers la l?gislation "
                              "et la jurisprudence "
                              "de l'Union europ?enne et mettra des liens vers les dispositions cit?s."
                              " Cela peut prendre un certain temps.Veuillez  confirmer!",

            # cPlzConfirmRmLnk
            "ConfirmRemoveAutoLink": "Veuillez confirmer la suppression des liens ?tablis par EULinksChecker.",

            "LongCitate": "Show long citation",

            "Citation": "Citation",

            "ShortCitate": "Show short citation",

            "CheckingForLinks": "V?rification des liens..."
        }

    de_map = \
        {
            # cAll
            "All": "Alles",

            # cCancel
            "Cancel": "Stornieren",

            # cCannotAddLink
            "MarkTextForInsert": "Bitte, markieren Sie einen Text um einen Link einzuf?gen.",

            # cConfirmation
            "Confirm": "Best?tigung",

            # cErrorOccured
            "ErrorOccured": "Fehler aufgetreten.",

            # cInsertLink
            "InsertLink": "Link einf?gen",

            # cm_AllDocuments
            "AllDocuments": "Alle Dokumente",

            # cm_DocumentsIndexedWithThisTerm
            "IndexedWithTerm": "Dokumente mit diesem Begriff indiziert:",

            # cm_DocumentsReferringToThisAct
            "ReferingWithProvision": "Dokumente, die auf diesen Akt / Vorschrift verweisen",

            # cm_EuCL
            "EuLaw": "EU-Rechtsprechung",

            # cm_EuL
            "EuLegislation": "EU-Gesetzgebung",

            # cm_NaCL
            "NationalLaw": "Nationale Rechtsprechung",

            # cm_NaL
            "NationalLegislation": "Nationale Gesetzgebung",

            # cm_RemoveLink
            "RemoveLink": "Link entfernen",

            # cm_ShowToolTip
            "ShowToolTip": "Hinweis anzeigen",

            # cMsgNoLinksSetByELCFound
            "NoLinksFound": "Keine Links vom EULinksChecker gefunden.",

            # cOK
            "OK": "Best?tigen",

            # cPlsSelLink
            "MarkAutomatedLink": "Bitte, w?hlen Sie einen Text mit Links gesetzt vom EULinksChecker.",

            # cPlzConfirm
            "ConfirmLinking": "EULinksChecker wird ?berpr?fen ob Verweise auf EU-Gesetzgebung oder Rechtsprechung "
                              " vorhanden sind und wird Links zu den zitierten Rechtsakten einf?gen. "
                              "Dies kann eine Weile dauern. Bitte, best?tigen!",

            # cPlzConfirmRmLnk
            "ConfirmRemoveAutoLink": "Bitte best?tigen Sie die Entfernung von Links gesetzt von EULinksCheker.",

            "LongCitate": "Show long citation",

            "Citation": "Citation",

            "ShortCitate": "Show short citation",

             "CheckingForLinks": "?berpr?fen auf Links..."
        }

    it_map = \
        {
            # cAll
            "All": "Tutto",

            # cCancel
            "Cancel": "Cancellare",

            # cCannotAddLink
            "MarkTextForInsert": "Si prega di selezionare testo per inserire un collegamento.",

            # cConfirmation
            "Confirm": "Conferma",

            # cErrorOccured
            "ErrorOccured": "Errore.",

            # cInsertLink
            "InsertLink": "Inserisci collegamento",

            # cm_AllDocuments
            "AllDocuments": "Tutti i documenti",

            # cm_DocumentsIndexedWithThisTerm
            "IndexedWithTerm": "I documenti indicizzati con questo termine:",

            # cm_DocumentsReferringToThisAct
            "ReferingWithProvision": "Documenti relativi a questo atto / disposizione:",

            # cm_EuCL
            "EuLaw": "Giurisprudenza dell'UE",

            # cm_EuL
            "EuLegislation": "Legislazione UE",

            # cm_NaCL
            "NationalLaw": "Giurisprudenza nazionale",

            # cm_NaL
            "NationalLegislation": "Legislazione Nazionale",

            # cm_RemoveLink
            "RemoveLink": "",

            # cm_ShowToolTip
            "ShowToolTip": "Mostra descrizione comandi",

            # cMsgNoLinksSetByELCFound
            "NoLinksFound": "Nessun collegamento messo dal EULinksChecker e stato trovato.",

            # cOK
            "OK": "OK",

            # cPlsSelLink
            "MarkAutomatedLink": "Si prega di selezionare testo con collegamenti messi dal EULinksChecker.",

            # cPlzConfirm
            "ConfirmLinking": "EULinksChecker verificher? la presenza di riferimenti alla legislazione e alla "
                              "giurisprudenza"
                              " dell'UE e metter? collegamenti ai testi normativi citati. "
                              "Questo potrebbe richiedere del tempo. "
                              "Si prega di confermare!",

            # cPlzConfirmRmLnk
            "ConfirmRemoveAutoLink": "Si prega di confermare l'eliminazione dеi collegamenti messi dal EULinksChecker.",

            "LongCitate": "Show long citation",

            "Citation": "Citation",

            "ShortCitate": "Show short citation",

            "CheckingForLinks": "Controllo per i collegamenti..."
        }

    def __init__(self):
        pass

    @staticmethod
    def all():
        lang, code = locale.getdefaultlocale()
        lang = 'en'
        result = ResourceManager.en_map['All']
        if lang.startswith('de'):
            result = ResourceManager.de_map['All']
        if lang.startswith('fr'):
            result = ResourceManager.fr_map['All']
        if lang.startswith('it'):
            result = ResourceManager.it_map['All']
        if lang.startswith('bg'):
            result = ResourceManager.bg_map['All']

        return result

    @staticmethod
    def confirm_linkig():
        return ResourceManager.en_map['ConfirmLinking']

    @staticmethod
    def confirm_remove_all_links():
        return ResourceManager.en_map['ConfirmRemoveAutoLink']

    @staticmethod
    def mark_text_for_insert():
        return ResourceManager.en_map['MarkTextForInsert']

    @staticmethod
    def mark_text_for_delete():
        return ResourceManager.en_map['MarkAutomatedLink']


    @staticmethod
    def input_url():
        return ResourceManager.en_map['EnterUrl']

    @staticmethod
    def error():
        return ResourceManager.en_map['ErrorOccured']

    @staticmethod
    def long_citation():
        return ResourceManager.en_map['LongCitate']

    @staticmethod
    def short_citation():
        return ResourceManager.en_map['ShortCitate']

    @staticmethod
    def citation():
        return ResourceManager.en_map['Citation']

    @staticmethod
    def working():
        return ResourceManager.en_map['CheckingForLinks']

    @staticmethod
    def unsupported_language():
        return ResourceManager.en_map['UnsupportedLang']

    @staticmethod
    def show_tooltip():
        return ResourceManager.en_map['ShowToolTip']


class Constant:
    def __init__(self):
        pass

    UI_ContextMenu_EN_ReferingActOrProvision = 'Documents refering this act (provision)'
    UI_ContextMenu_EN_NationalCaseLaw = 'National case law'
    UI_ContextMenu_EN_NationalLegislation = 'National legislation'
    UI_ContextMenu_EN_EUCaseLaw = 'European case law'
    UI_ContextMenu_EN_EULegislation = 'European legislation'
    UI_ContextMenu_EN_All = 'All'
    UI_ContextMenu_EN_ShowHint = 'Show hint'

    UI_ContextMenu_EN_IndexedTerms = 'Documents indexed with this term'

    UI_ContextMenu_EN_Remove = 'Remove'

    EurocasesRef_BaseUrl = 'http://demo.eurocases.eu/api/Doc/DocInLinks/'
    EurocasesHint_BaseUrl = 'http://demo.eurocases.eu/api/Doc/ParHint/4/4/'
    EurocasesTerm_BaesUrl = 'http://demo.eurocases.eu/api/Doc/SearchByXmlId/'
    EurocasesRefType_NationalCaseType = 'NatCL'
    EurocasesRefType_NationalLegislationType = 'NatL'
    EurocasesRefType_EuCaseType = 'EuCL'
    EurocasesRefType_EuLegislationType = 'EuL'
    EurocasesRefType_AllType = 'All'

    Command_Ref_RelevantNationalCaseLaw = 'service:org.openoffice.comp.pyuno.eucases.ShowRelevantNationalCaseRef?insert'
    Command_Ref_RelevantNationalLegislation = \
        'service:org.openoffice.comp.pyuno.eucases.ShowRelevantNationalLegislationRef?insert'
    Command_Ref_RelevantEuCase = 'service:org.openoffice.comp.pyuno.eucases.ShowRelevantEuCaseRef?insert'
    Commanad_Ref_RelevantEuLegislation = 'service:org.openoffice.comp.pyuno.eucases.ShowRelevantEuLegislationRef?insert'
    Command_Ref_AllRelevant = 'service:org.openoffice.comp.pyuno.eucases.ShowAllRelevantRef?insert'

    Command_Hint_Show = 'service:org.openoffice.comp.pyuno.eucases.ShowHint?insert'

    Command_Term_RelevantNationalCaseLaw = \
        'service:org.openoffice.comp.pyuno.eucases.ShowRelevantNationalCaseTerm?insert'
    Command_Term_RelevantNationalLegislation = \
        'service:org.openoffice.comp.pyuno.eucases.ShowRelevantNationalLegislationTerm?insert'
    Command_Term_RelevantEuCaseLaw = 'service:org.openoffice.comp.pyuno.eucases.ShowRelevantEuCaseLawTerm?insert'
    Command_Term_RelevantEULegislation = \
        'service:org.openoffice.comp.pyuno.eucases.ShowRelevantEuLegislationTerm?insert'
    Command_Term_AllRelevant = 'service:org.openoffice.comp.pyuno.eucases.ShowAllRelevantTerm?insert'
    Command_Remove_Hyperlink = 'service:org.openoffice.comp.pyuno.eucases.RemoveSelectedLink?insert'
    Command_Long_Citation = 'service:org.openoffice.comp.pyuno.eucases.GetLongCitate?insert'
    Command_Short_Citation = 'service:org.openoffice.comp.pyuno.eucases.GetShortCitate?insert'

    Green_Color = 32768
    _Red_Color = 16711680
    Red_Color = 13395456
    FullCitate = 2
    ShortCitate = 1

    ControlService_DialogModel = 'com.sun.star.awt.UnoControlDialogModel'
    ControlService_Dialog = 'com.sun.star.awt.UnoControlDialog'
    ControlService_Toolkit = 'com.sun.star.awt.Toolkit'
    ControlService_ButtonModel = 'com.sun.star.awt.UnoControlButtonModel'
    ControlService_LabelModel = 'com.sun.star.awt.UnoControlFixedTextModel'
    ControlService_Input = 'com.sun.star.awt.UnoControlEditModel'

    Warnning = 'Warnning'
    Ok = 'Ok'
    No_Selection = 'No selection'
    Insert_Url = 'Insert url'

    Btn_Name_Ok = 'btn_ok'
    Label_Name_No_Selection = 'label_no_selection'

    Tmp_Html_File = 'file:///C:/Windows/Temp/ec-tmp.html'
    Tmp_Html_File_Read = 'C:\\Windows\\Temp\\ec-tmp.html'
    Tmp_Doc_File = 'file:///C:/Windows/Temp/ec-tmp.doc'
    Tmp_Doc_File_Read = 'C:\\Windows\\Temp\\ec-tmp.doc'




g_ImplementationHelper = unohelper.ImplementationHelper()

g_ImplementationHelper.addImplementation(JobExecutor, "grammalecte.ContextMenuHandler", ("grammalecte.ContextMenuHandler",),)

g_ImplementationHelper.addImplementation(AddLinksJob, "org.openoffice.comp.pyuno.eucases.AddLinks",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(RemoveSelectedLinkJob, "org.openoffice.comp.pyuno.eucases.RemoveSelectedLink",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowRelevantNationalCaseRefJob,
                                         "org.openoffice.comp.pyuno.eucases.ShowRelevantNationalCaseRef",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowRelevantNationalLegislationRefJob,
                                         "org.openoffice.comp.pyuno.eucases.ShowRelevantNationalLegislationRef",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowRelevantEuCaseRefJob,
                                         "org.openoffice.comp.pyuno.eucases.ShowRelevantEuCaseRef",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowRelevantEuLegislationRefJob,
                                         "org.openoffice.comp.pyuno.eucases.ShowRelevantEuLegislationRef",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowAllRelevantRefJob, "org.openoffice.comp.pyuno.eucases.ShowAllRelevantRef",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowHintJob, "org.openoffice.comp.pyuno.eucases.ShowHint",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowRelevantNationalCaseTermJob,
                                         "org.openoffice.comp.pyuno.eucases.ShowRelevantNationalCaseTerm",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowRelevantNationalLegislationTermJob,
                                         "org.openoffice.comp.pyuno.eucases.ShowRelevantNationalLegislationTerm",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowRelevantEuCaseLawTermJob,
                                         "org.openoffice.comp.pyuno.eucases.ShowRelevantEuCaseLawTerm",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowRelevantEuLegislationTermJob,
                                         "org.openoffice.comp.pyuno.eucases.ShowRelevantEuLegislationTerm",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ShowAllRelevantTermJob,
                                         "org.openoffice.comp.pyuno.eucases.ShowAllRelevantTerm",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(RemoveSelectedLinkJob,
                                         "org.openoffice.comp.pyuno.eucases.RemoveSelectedLink",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(RemoveAllLinksJob,
                                         "org.openoffice.comp.pyuno.eucases.RemoveAllLinks",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(RemoveLinksFromSelectionJob,
                                         "org.openoffice.comp.pyuno.eucases.RemoveLinksFromSelection",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(InsertLinkIntoSelectionJob,
                                         "org.openoffice.comp.pyuno.eucases.InsertLinkIntoSelection",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(ExportToXmlJob,
                                         "org.openoffice.comp.pyuno.eucases.ExportToXml",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(GetLongCitateJob,
                                         "org.openoffice.comp.pyuno.eucases.GetLongCitate",
                                         ("com.sun.star.task.Job",),)

g_ImplementationHelper.addImplementation(GetShortCitateJob,
                                         "org.openoffice.comp.pyuno.eucases.GetShortCitate",
                                         ("com.sun.star.task.Job",),)



