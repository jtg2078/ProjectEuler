using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.Utils;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Managers;
using System;
using Microsoft.Xna.Framework.Input;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlCombobox : MyGuiControlBase
    {
        class MyGuiControlComboboxItem : IComparable
        {
            public int Key;
            public Texture2D Icon;
            public StringBuilder Value;
            public int SortOrder;

            //  Sorts from small to large, e.g. 0, 1, 2, 3, ...
            public int CompareTo(object compareToObject)
            {
                MyGuiControlComboboxItem compareToItem = (MyGuiControlComboboxItem)compareToObject;
                return this.SortOrder.CompareTo(compareToItem.SortOrder);
            }
        }

        public delegate void OnComboBoxSelectCallback();
        public OnComboBoxSelectCallback OnSelect;

        Vector4 m_textColor;
        float m_textScale;
        bool m_isOpen;
        List<MyGuiControlComboboxItem> m_items;
        MyGuiControlComboboxItem m_selected;                            //  Item that is selected in the combobox, that is displayed in the main rectangle
        MyGuiControlComboboxItem m_preselectedMouseOver;                //  Item that is under mouse and may be selected if user clicks on it
        MyGuiControlComboboxItem m_preselectedMouseOverPrevious;        //  Same as m_preselectedMouseOver, but in previous update
        int? m_preselectedKeyboardIndex = null;                                 //  Same as m_preselectedMouseOver, but for keyboard. By default no item is selected.
        int? m_preselectedKeyboardIndexPrevious = null;                         //  Same as m_preselectedMouseOverPrevious, but for keyboard

        //  Scroll Bar logic code
        int m_openAreaItemsCount = 2;
        int DISPLAY_ITEM_MIDDLE_INDEX = 2 / 2 - 1;
        bool m_showScrollBar;
        float? m_scrollBarCurrentPosition;
        float m_minScrollBarPosition;
        float m_maxScrollBarPosition;
        float m_scrollBarStartPositionAbs;
        float m_scrollBarEndPositionAbs;
        float m_scrollBarEndPositionRelative;
        int m_displayItemsStartIndex;
        int m_displayItemsEndIndex;
        int m_scrollBarItemOffSet;
        float m_scrollBarHeight;
        float m_scrollBarWidth; // not the texture width, but the clickable area width
        float m_scrollBarPadding = 0.022f;
        float m_comboboxItemDeltaHeight = MyGuiConstants.COMBOBOX_ITEMS_DELTA_Y * 5;
        float m_someOffSetThingy = 0.007f;
        Vector2 m_someOffSetThingy2 = new Vector2(0, 0.002f);
        Texture2D comboboxItemIcon;
        bool m_supportPicture;
        bool m_supportSmoothScroll;
        Vector2 combobix_icon_offset = new Vector2(0.01f, 0.01f);
        float m_Ratio;


        public MyGuiControlCombobox(MyGuiScreenBase parentScreen, Vector2 position, float width, Vector4 textColor, float textScale)
            : base(parentScreen, position, new Vector2(width, MyGuiConstants.COMBOBOX_HEIGHT), MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.BUTTON_BORDER_COLOR)
        {
            m_canHandleKeyboardActiveControl = true;
            m_items = new List<MyGuiControlComboboxItem>();
            m_isOpen = false;
            m_textColor = textColor;
            m_textScale = textScale;
            InitializeScrollBarParameters();
        }

        public MyGuiControlCombobox(bool supportPicture, Texture2D initialTexture, MyGuiScreenBase parentScreen, Vector2 position, float width, float height, Vector4 textColor, float textScale, MyGuiDrawAlignEnum align)
            : base(parentScreen, MyGuiManager.GetAlignedCoordinate(position, new Vector2(width, height), align), new Vector2(width, height), MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.BUTTON_BORDER_COLOR)
        {
            m_supportPicture = supportPicture;
            comboboxItemIcon = initialTexture;
            m_canHandleKeyboardActiveControl = true;
            m_items = new List<MyGuiControlComboboxItem>();
            m_isOpen = false;
            m_textColor = textColor;
            m_textScale = textScale;
            InitializeScrollBarParameters();
        }

        public MyGuiControlCombobox(MyGuiScreenBase parentScreen, Vector2 position, float width, float height, Vector4 textColor, float textScale, int openAreaItemsCount, bool supportPicture, bool supportSmoothScroll)
            : base(parentScreen, position, new Vector2(width, height), MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.BUTTON_BORDER_COLOR)
        {
            m_canHandleKeyboardActiveControl = true;
            m_items = new List<MyGuiControlComboboxItem>();
            m_isOpen = false;
            m_textColor = textColor;
            m_textScale = textScale;
            m_openAreaItemsCount = openAreaItemsCount;
            DISPLAY_ITEM_MIDDLE_INDEX = m_openAreaItemsCount / 2 - 1;
            m_supportPicture = supportPicture;
            m_supportSmoothScroll = supportSmoothScroll;
            InitializeScrollBarParameters();
        }

        //  Clears/removes all items
        public void ClearItems()
        {
            m_items.Clear();
            InitializeScrollBarParameters();
        }

        //  Same as other AddItem, but this one auto-assign sort order
        public void AddItem(int key, MyTextsWrapperEnum value)
        {
            AddItem(key, value, m_items.Count);
        }

        //  Add new item
        public void AddItem(int key, MyTextsWrapperEnum value, int sortOrder)
        {
            AddItem(key, MyTextsWrapper.Get(value), sortOrder);
        }

        //  Add new item
        public void AddItem(int key, StringBuilder value)
        {
            AddItem(key, value, m_items.Count);
        }

        // Add new item
        public void AddItem(int key, Texture2D icon, StringBuilder value)
        {
            AddItem(key, icon, value, m_items.Count);
        }

        //  Add new item
        public void AddItem(int key, StringBuilder value, int sortOrder)
        {
            //  Create new item
            MyGuiControlComboboxItem newItem = new MyGuiControlComboboxItem();
            newItem.Key = key;
            newItem.Value = value;
            newItem.SortOrder = sortOrder;
            //  Add to list
            m_items.Add(newItem);

            //  Reorder the list
            m_items.Sort();

            //  scroll bar parameters need to be recalculated when new item is added
            AdjustScrollBarParameters();
        }

        //  Add new item
        public void AddItem(int key, Texture2D icon, StringBuilder value, int sortOrder)
        {
            //  Create new item
            MyGuiControlComboboxItem newItem = new MyGuiControlComboboxItem();
            newItem.Key = key;
            newItem.Icon = icon;
            newItem.Value = value;
            newItem.SortOrder = sortOrder;
            //  Add to list
            m_items.Add(newItem);

            //  Reorder the list
            m_items.Sort();

            //  scroll bar parameters need to be recalculated when new item is added
            AdjustScrollBarParameters();
        }

        public bool IsOpen()
        {
            return m_isOpen;
        }

        //  Selects item by index, so when you want to make first item as selected call SelectItemByIndex(0)
        public void SelectItemByIndex(int index)
        {
            m_selected = m_items[index];
        }

        //  Selects item by key
        public void SelectItemByKey(int key)
        {
            for (int i = 0; i < m_items.Count; i++)
            {
                MyGuiControlComboboxItem item = m_items[i];

                if (item.Key.Equals(key))
                {
                    m_selected = item;
                    m_preselectedKeyboardIndex = i;
                    if (m_showScrollBar == true) ScrollToPreSelectedItem();
                    if (OnSelect != null) OnSelect();
                    return;
                }
            }
        }

        //  Return key of selected item
        public int GetSelectedKey()
        {
            return m_selected.Key;
        }

        //  Return value of selected item
        public StringBuilder GetSelectedValue()
        {
            return m_selected.Value;
        }

        void Assert()
        {
            //  If you forget to set default or pre-selected item, you must do it! It won't be assigned automaticaly!
            MyMwcUtils.AssertRelease(m_selected != null);

            //  Combobox can't be empty!
            MyMwcUtils.AssertRelease(m_items.Count > 0);
        }

        //  Method returns true if input was captured by control, so no other controls, nor screen can use input in this update
        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            bool ret = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);

            Assert();

            if (ret == false)
            {
                if ((IsMouseOver() == true) && (input.IsNewLeftMousePressed() == true))
                {
                    MyGuiSounds.PlayClick();
                    m_isOpen = !m_isOpen;
                }

                if ((hasKeyboardActiveControl == true) && ((input.IsNewKeyPress(Keys.Enter)) || (input.IsNewKeyPress(Keys.Space))))
                {
                    MyGuiSounds.PlayClick();

                    if ((m_preselectedKeyboardIndex.HasValue) && (m_preselectedKeyboardIndex.Value < m_items.Count))
                    {
                        SelectItemByKey(m_items[m_preselectedKeyboardIndex.Value].Key);
                    }

                    //  Close but capture focus for this update so parent screen don't receive this ENTER
                    m_isOpen = !m_isOpen;
                    //ret = true;
                }

                //  If combobox is open, then this control has captured the focus and parent screen can't handle it until we close the combobox
                ret = (m_isOpen == true);

                if (m_isOpen == true)
                {
                    if (m_showScrollBar == true)
                    {
                        //  Handles mouse input of dragging the scrollbar up or down
                        Vector2 position = GetDrawPosition();
                        float minX = position.X + m_size.Value.X - m_scrollBarWidth;
                        float maxX = position.X + m_size.Value.X;
                        float minY = position.Y + m_comboboxItemDeltaHeight;
                        if ((MyGuiManager.MouseCursorPosition.X >= minX) && (MyGuiManager.MouseCursorPosition.X <= maxX)
                            && (input.IsLeftMousePressed() == true) && (MyGuiManager.MouseCursorPosition.Y >= minY))
                        {
                            SetScrollBarPosition(MyGuiManager.MouseCursorPosition.Y - (m_scrollBarStartPositionAbs + m_comboboxItemDeltaHeight));
                        }
                    }

                    //  If ESC was pressed while combobox has keyboard focus and combobox was opened, then close combobox but don't send this ESC to parent screen
                    //  Or if user clicked outside of the combobox's area
                    if (((hasKeyboardActiveControl == true) && (input.IsNewKeyPress(Keys.Escape))) ||
                        ((IsMouseOverOnOpenedArea(input) != true) && (IsMouseOver() != true) && (input.IsNewLeftMousePressed() == true)))
                    {
                        MyGuiSounds.PlayClick();
                        m_isOpen = false;

                        //  Still capture focus, don't allow parent screen to receive this ESCAPE
                        ret = true;
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //  Mouse controling items in the combobox
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //  Search for item that is under the mouse cursor
                    m_preselectedMouseOverPrevious = m_preselectedMouseOver;
                    m_preselectedMouseOver = null;

                    //  The following are used for controlling scroll window range
                    int startIndex = 0;
                    int endIndex = m_items.Count;
                    float widthOffSet = 0f;
                    if (m_showScrollBar == true)
                    {
                        startIndex = m_displayItemsStartIndex;
                        endIndex = m_displayItemsEndIndex;
                        widthOffSet = m_scrollBarWidth;
                    }

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        Vector2 position = GetOpenItemPosition(i - m_displayItemsStartIndex);
                        Vector2 min = position + new Vector2(0, -m_comboboxItemDeltaHeight / 2.0f);
                        Vector2 max = min + new Vector2(m_size.Value.X - widthOffSet, m_comboboxItemDeltaHeight);

                        if ((MyGuiManager.MouseCursorPosition.X >= min.X) && (MyGuiManager.MouseCursorPosition.X <= max.X) && (MyGuiManager.MouseCursorPosition.Y >= min.Y) && (MyGuiManager.MouseCursorPosition.Y <= max.Y))
                        {
                            m_preselectedMouseOver = m_items[i];
                        }
                    }

                    if (m_preselectedMouseOver != m_preselectedMouseOverPrevious)
                    {
                        MyGuiSounds.PlayMouseOver();
                    }

                    //  Select item when user clicks on it
                    if ((m_preselectedMouseOver != null) && (input.IsNewLeftMousePressed() == true))
                    {
                        //m_selected = m_preselectedMouseOver;
                        SelectItemByKey(m_preselectedMouseOver.Key);

                        MyGuiSounds.PlayClick();
                        m_isOpen = false;

                        //  Still capture focus, don't allow parent screen to receive this CLICK
                        ret = true;
                    }


                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //  Keyboard controling items in the combobox
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //  Keyboard movement
                    if (input.IsNewKeyPress(Keys.Down))
                    {
                        HandleKeyboard(true);
                        ret = true;
                    }
                    else if (input.IsNewKeyPress(Keys.Up))
                    {
                        HandleKeyboard(false);
                        ret = true;
                    }
                    else if (input.IsNewKeyPress(Keys.Tab))
                    {
                        //  We want to close the combobox without selecting any item and forward TAB or SHIF+TAB to parent screen so it can navigate to next control
                        m_isOpen = false;
                        ret = false;
                    }
                }
            }

            return ret;
        }

        //  Moves keyboard index to the next item, or previous item, or first item in the combobox.
        //  forwardMovement -> set to TRUE when you want forward movement, set to FALSE when you wasnt backward
        void HandleKeyboard(bool forwardMovement)
        {
            m_preselectedKeyboardIndexPrevious = m_preselectedKeyboardIndex;

            int sign = (forwardMovement == true) ? +1 : -1;

            if (m_preselectedKeyboardIndex.HasValue == false)
            {
                //  If this is first keypress in this combobox, we will set keyboard index to begining or end of the list
                m_preselectedKeyboardIndex = (forwardMovement == true) ? 0 : m_items.Count - 1;
            }
            else
            {
                //  Increase or decrease and than check ranges and do sort of overflow
                m_preselectedKeyboardIndex += sign;
                if (m_preselectedKeyboardIndex > (m_items.Count - 1)) m_preselectedKeyboardIndex = 0;
                if (m_preselectedKeyboardIndex < 0) m_preselectedKeyboardIndex = m_items.Count - 1;
            }

            if (m_preselectedKeyboardIndex != m_preselectedKeyboardIndexPrevious)
            {
                MyGuiSounds.PlayMouseOver();
            }

            //  Programmatically adjust the scroll bar position based on changes in m_preselectedKeyboardIndex
            //  So it handles the scrolling action when users press up and down keys
            if (m_showScrollBar == true)
            {
                //  These two conditions handle when either
                //  1. the index is at top of the display index range (so scrolls up)
                //  2. the index is at bottom of the display index range (so scrolls down)
                //  3. if neither, then the index is in between the display range, so no scrolling is needed yet
                if (m_preselectedKeyboardIndex >= m_displayItemsEndIndex)
                {
                    m_displayItemsEndIndex = Math.Max(m_openAreaItemsCount, m_preselectedKeyboardIndex.Value + 1);
                    m_displayItemsStartIndex = Math.Max(0, m_displayItemsEndIndex - m_openAreaItemsCount);
                    m_scrollBarCurrentPosition = m_displayItemsStartIndex * m_maxScrollBarPosition / m_scrollBarItemOffSet;
                }
                else if (m_preselectedKeyboardIndex <= m_displayItemsStartIndex)
                {
                    m_displayItemsStartIndex = Math.Max(0, m_preselectedKeyboardIndex.Value);
                    m_displayItemsEndIndex = Math.Max(m_openAreaItemsCount, m_displayItemsStartIndex + m_openAreaItemsCount);
                    m_scrollBarCurrentPosition = m_displayItemsStartIndex * m_maxScrollBarPosition / m_scrollBarItemOffSet;
                }
            }
        }

        //  Checks if mouse cursor is over opened combobox area
        bool IsMouseOverOnOpenedArea(MyGuiInput input)
        {
            MyRectangle2D openedArea = GetOpenedArea();

            Vector2 min = openedArea.LeftTop;
            Vector2 max = openedArea.LeftTop + openedArea.Size;

            return ((MyGuiManager.MouseCursorPosition.X >= min.X) && (MyGuiManager.MouseCursorPosition.X <= max.X) && (MyGuiManager.MouseCursorPosition.Y >= min.Y) && (MyGuiManager.MouseCursorPosition.Y <= max.Y));
        }

        MyRectangle2D GetOpenedArea()
        {
            MyRectangle2D ret;
            ret.LeftTop = m_parentScreen.GetPosition() + m_position + new Vector2(-m_size.Value.X / 2.0f, m_size.Value.Y / 2.0f);

            //  Adjust the open area to be as big as the scroll bar MAX_VISIBLE_ITEMS_COUNT when scrollbar is on
            if (m_showScrollBar == true)
                ret.Size = new Vector2(m_size.Value.X, m_openAreaItemsCount * m_comboboxItemDeltaHeight);
            else
                ret.Size = new Vector2(m_size.Value.X, m_items.Count * m_comboboxItemDeltaHeight + m_comboboxItemDeltaHeight * 0.2f + m_someOffSetThingy);

            return ret;
        }

        //  Returns position of item in open list
        Vector2 GetOpenItemPosition(int index)
        {
            if (m_supportSmoothScroll == true) index--;

            return GetDrawPosition() + new Vector2(0, m_size.Value.Y / 2.0f + index * m_comboboxItemDeltaHeight);
        }

        Vector2 GetDrawPosition()
        {
            return m_parentScreen.GetPosition() + m_position + new Vector2(-m_size.Value.X / 2.0f, 0);
        }

        public override void Draw()
        {
            base.Draw();

            Assert();



            MyGuiManager.SpriteBatch.Begin();

            Vector2 position = GetDrawPosition();

            Vector4 tempTextColor = (IsMouseOverOrKeyboardActive() == false) ? m_textColor : m_textColor * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER;

            Vector4 tempBorderColor = (IsMouseOverOrKeyboardActive() == false) ? m_borderColor.Value : m_borderColor.Value * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER;

            if (m_supportPicture == true)
            {
                var anotherOffSet = new Vector2(-0.01f, (m_comboboxItemDeltaHeight - 0.02f) / 2);

                MyGuiManager.DrawSpriteBatch(m_items[0].Icon, position - anotherOffSet,
                            new Vector2(m_comboboxItemDeltaHeight - 0.02f, m_comboboxItemDeltaHeight - 0.02f), new Color(tempBorderColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                var txtPosition = position + new Vector2(m_comboboxItemDeltaHeight - 0.01f, -(m_comboboxItemDeltaHeight - 0.02f) / 2 + 0.005f);
                var TEXT_SCALE = 0.75f; //0.75f
                var moveDownOffSet = new Vector2(0, MyGuiConstants.COMBOBOX_ITEMS_DELTA_Y);
                MyGuiManager.DrawString(MyGuiManager.FontGuiImpactLarge,
                    new StringBuilder("Cactuars are usually very fast, "),
                    txtPosition + MyGuiConstants.COMBOBOX_TEXT_OFFSET, TEXT_SCALE,
                    GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                //txtPosition += moveDownOffSet;
                //MyGuiManager.DrawString(MyGuiManager.FontGuiImpactLarge,
                //    new StringBuilder("and are tough to hit. Their HP "),
                //    txtPosition  + MyGuiConstants.COMBOBOX_TEXT_OFFSET, TEXT_SCALE,
                //    GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                //txtPosition += moveDownOffSet;

                //MyGuiManager.DrawString(MyGuiManager.FontGuiImpactLarge,
                //    new StringBuilder("is also usually very low, but their"),
                //    txtPosition + MyGuiConstants.COMBOBOX_TEXT_OFFSET, TEXT_SCALE,
                //    GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                //txtPosition += moveDownOffSet;

                //MyGuiManager.DrawString(MyGuiManager.FontGuiImpactLarge,
                //    new StringBuilder("defenses are high."),
                //    txtPosition + MyGuiConstants.COMBOBOX_TEXT_OFFSET, TEXT_SCALE,
                //    GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }
            else
            {
                //  Selected item
                MyGuiManager.DrawString(MyGuiManager.FontGuiImpactLarge, m_selected.Value, position + MyGuiConstants.COMBOBOX_TEXT_OFFSET, m_textScale,
                    GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            }

            //  Vertical line at end right end
            MyGuiManager.DrawSpriteBatch(MyGuiManager.BlankTexture, new Vector2(position.X + m_size.Value.X - 0.025f, position.Y), 1, m_size.Value.Y * 0.8f,
                GetColorAfterTransitionAlpha(tempBorderColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

            //  Arrow sprite ar the right end
            MyGuiManager.DrawSpriteBatch(MyGuiManager.ComboboxTexture, new Vector2(position.X + m_size.Value.X - 0.0125f, position.Y), 0.5f,
                GetColorAfterTransitionAlpha(tempBorderColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, 0, Vector2.Zero);


            MyGuiManager.SpriteBatch.End();

            //  Items from the open list
            if (m_isOpen == true)
            {
                DrawBackgroundToStencil(GetOpenedArea());
                StencilOpaqueEnable();
                MyGuiManager.SpriteBatch.Begin();
                MyRectangle2D openedArea = GetOpenedArea();

                //  Draw background for items - we use DrawBackgroundRectangleForScreen() only because it can draw 
                //  different texture according to aspect ratio of the rectangle
                MyGuiManager.DrawBackgroundRectangleForScreen(openedArea.LeftTop, openedArea.Size, GetColorAfterTransitionAlpha(m_backgroundColor.Value * 1.5f),
                    GetColorAfterTransitionAlpha(m_borderColor.Value), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                //DrawBackgroundToStencil(openedArea);

                //  Draw the scroll bar
                if (m_showScrollBar == true)
                {
                    MyGuiManager.DrawSpriteBatch(MyGuiManager.ScrollbarSlider,
                        new Vector2(position.X + m_size.Value.X - 0.0125f, openedArea.LeftTop.Y + m_scrollBarCurrentPosition.Value),
                        new Vector2(0.0125f, m_scrollBarHeight), new Color(tempBorderColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
                }

                //  The following are used for controlling scroll window range
                int startIndex = 0;
                int endIndex = m_items.Count;
                float widthOffSet = 0f;
                if (m_showScrollBar == true)
                {
                    startIndex = m_displayItemsStartIndex;
                    endIndex = m_displayItemsEndIndex;
                    widthOffSet = m_scrollBarWidth;
                }

                //  Draw items
                for (int i = startIndex; i < endIndex; i++)
                {
                    MyGuiControlComboboxItem item = m_items[i];

                    Vector2 itemPosition = GetOpenItemPosition(i - m_displayItemsStartIndex) + new Vector2(0, (m_Ratio - m_displayItemsStartIndex) * m_comboboxItemDeltaHeight);
                    float textScale;

                    if ((item == m_preselectedMouseOver) || ((m_preselectedKeyboardIndex.HasValue) && (m_preselectedKeyboardIndex == i)))
                    {
                        textScale = m_textScale * MyGuiConstants.BUTTON_MOUSE_OVER_TEXT_SCALE;

                        //  Background for highlighting the area where mouse cursor is
                        MyGuiManager.DrawBackgroundRectangleForControl(itemPosition + m_someOffSetThingy2, new Vector2(m_size.Value.X - widthOffSet, m_comboboxItemDeltaHeight),
                            GetColorAfterTransitionAlpha(m_backgroundColor.Value * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER),
                            GetColorAfterTransitionAlpha(m_borderColor.Value), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                    }
                    else
                    {
                        textScale = m_textScale;
                    }
                    if (m_supportPicture == true)
                    {
                        MyGuiManager.DrawSpriteBatch(item.Icon, itemPosition + combobix_icon_offset,
                                new Vector2(m_comboboxItemDeltaHeight - 0.02f, m_comboboxItemDeltaHeight - 0.02f), new Color(tempBorderColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                        var txtPosition = itemPosition + new Vector2(m_comboboxItemDeltaHeight - 0.01f, 0.005f);
                        var TEXT_SCALE = 0.75f; //0.75f
                        var moveDownOffSet = new Vector2(0, MyGuiConstants.COMBOBOX_ITEMS_DELTA_Y);
                        MyGuiManager.DrawString(MyGuiManager.FontGuiImpactLarge,
                            item.Value,
                            txtPosition + MyGuiConstants.COMBOBOX_TEXT_OFFSET, TEXT_SCALE,
                            GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);


                        //if (i == 0 || i==1)
                        //{
                        //    MyGuiManager.DrawSpriteBatch(MyGuiManager.MexicanJedi, itemPosition + combobix_icon_offset,
                        //        new Vector2(m_comboboxItemDeltaHeight - 0.02f, m_comboboxItemDeltaHeight - 0.02f), new Color(tempBorderColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                        //    var txtPosition = itemPosition + new Vector2(m_comboboxItemDeltaHeight - 0.01f, 0.005f);
                        //    var TEXT_SCALE = 0.75f; //0.75f
                        //    var moveDownOffSet = new Vector2(0, MyGuiConstants.COMBOBOX_ITEMS_DELTA_Y);
                        //    MyGuiManager.DrawString(MyGuiManager.FontGuiImpactLarge,
                        //        new StringBuilder("soy tu padre..."),
                        //        txtPosition + MyGuiConstants.COMBOBOX_TEXT_OFFSET, TEXT_SCALE,
                        //        GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                        //    txtPosition += moveDownOffSet;
                        //    MyGuiManager.DrawString(MyGuiManager.FontGuiImpactLarge,
                        //        new StringBuilder("no mames guey"),
                        //        txtPosition + MyGuiConstants.COMBOBOX_TEXT_OFFSET, TEXT_SCALE,
                        //        GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                        //}
                        //if (i > 1)
                        //{
                        //    MyGuiManager.DrawSpriteBatch(MyGuiManager.Taco, itemPosition + combobix_icon_offset,
                        //        new Vector2(m_comboboxItemDeltaHeight - 0.02f, m_comboboxItemDeltaHeight - 0.02f), new Color(tempBorderColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                        //    var txtPosition = itemPosition + new Vector2(m_comboboxItemDeltaHeight - 0.01f, 0.005f);
                        //    var TEXT_SCALE = 0.75f; //0.75f
                        //    var moveDownOffSet = new Vector2(0, MyGuiConstants.COMBOBOX_ITEMS_DELTA_Y);
                        //    MyGuiManager.DrawString(MyGuiManager.FontGuiImpactLarge,
                        //        new StringBuilder("Taco"),
                        //        txtPosition + MyGuiConstants.COMBOBOX_TEXT_OFFSET, TEXT_SCALE,
                        //        GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                        //}
                    }
                    else
                    {
                        MyGuiManager.DrawString(MyGuiManager.FontGuiImpactLarge, item.Value, itemPosition + MyGuiConstants.COMBOBOX_TEXT_OFFSET,
                            textScale, GetColorAfterTransitionAlpha(m_textColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                    }

                }
                MyGuiManager.SpriteBatch.End();
                StencilOpaqueDisable();
            }

            MyRenderStatePool.RestoreAfterSpriteBatch();
        }

        //  Call this before you start drawing normal objects/scene. It will make sure that nothing will be drawn were stencil values are equal to 1.
        public void StencilOpaqueEnable()
        {
            //  Write only where stencil enables it
            MyRenderStatePool.StencilEnable = true;
            MyRenderStatePool.StencilFunction = CompareFunction.Equal;
            MyRenderStatePool.ReferenceStencil = 1;

            //  But disable writing to stencil buffer
            MyRenderStatePool.StencilPass = StencilOperation.Keep;
        }

        //  Call this after you drawn normal objects/scene
        public void StencilOpaqueDisable()
        {
            //  Turn off stencil and alpha-blending
            //MyRenderStatePool.AlphaBlendEnable = false;
            MyRenderStatePool.StencilEnable = false;
            MyRenderStatePool.StencilFunction = CompareFunction.Always;
        }

        public void DrawBackgroundToStencil(MyRectangle2D openedArea)
        {
            GraphicsDevice device = MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice;
            device.Clear(ClearOptions.Stencil, Color.TransparentBlack, 1.0f, 0);

            //  Now we are going to draw a swipe mask into the stencil buffer. Set these renderstates to write 1's into the stencil buffer
            //  Also, set these states to disable drawing to the regular color buffer
            MyRenderStatePool.StencilEnable = true;
            MyRenderStatePool.StencilFunction = CompareFunction.Always;
            MyRenderStatePool.StencilPass = StencilOperation.Replace;
            MyRenderStatePool.ReferenceStencil = 1;
            MyRenderStatePool.AlphaBlendEnable = true;
            MyRenderStatePool.SourceBlend = Blend.Zero;
            MyRenderStatePool.DestinationBlend = Blend.One;
            //MyRenderStatePool.DepthBufferWriteEnable = false;

            //  This will make sure that only pixels where texture contains alpha with value bigger than zero are drawn, therefore only there we write to stencil
            //MyRenderStatePool.AlphaFunction = CompareFunction.Greater;
            //MyRenderStatePool.AlphaTestEnable = true;
            //MyRenderStatePool.ReferenceAlpha = 0;
            MyGuiManager.SpriteBatch.Begin();

            MyGuiManager.DrawRectangleForScreen(openedArea.LeftTop, openedArea.Size, GetColorAfterTransitionAlpha(m_backgroundColor.Value * 1.5f),
                    GetColorAfterTransitionAlpha(m_borderColor.Value), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            MyGuiManager.SpriteBatch.End();
            //MyRenderStatePool.AlphaFunction = CompareFunction.Equal;

            MyRenderStatePool.AlphaTestEnable = false;
            MyRenderStatePool.AlphaBlendEnable = false;
            MyRenderStatePool.DepthBufferWriteEnable = true;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Scroll Bar logic code
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void InitializeScrollBarParameters()
        {
            //  Scroll bar size related
            m_scrollBarHeight = 0;
            m_scrollBarWidth = 0.025f;

            //  Scroll bar position and range related
            m_minScrollBarPosition = 0;
            m_maxScrollBarPosition = 0;
            m_scrollBarCurrentPosition = 0;
            m_scrollBarStartPositionAbs = GetOpenedArea().LeftTop.Y;
            m_scrollBarEndPositionAbs = 0;
            m_scrollBarEndPositionRelative = m_openAreaItemsCount * m_comboboxItemDeltaHeight - m_scrollBarPadding;

            //  Display items range index related
            m_displayItemsStartIndex = 0;
            m_displayItemsEndIndex = m_openAreaItemsCount;

            //  Misc
            m_showScrollBar = false;
            m_scrollBarItemOffSet = 0;
        }

        void AdjustScrollBarParameters()
        {
            m_showScrollBar = m_items.Count > m_openAreaItemsCount;
            if (m_showScrollBar == true)
            {
                m_scrollBarHeight = Math.Max(m_openAreaItemsCount * m_scrollBarEndPositionRelative / m_items.Count, m_comboboxItemDeltaHeight);
                //m_maxScrollBarPosition = MAX_VISIBLE_ITEMS_COUNT * m_comboboxItemDeltaHeight - (m_scrollBarHeight + m_scrollBarPadding);
                m_maxScrollBarPosition = (GetOpenedArea().LeftTop + GetOpenedArea().Size).Y - m_scrollBarHeight - m_scrollBarPadding;
                m_scrollBarItemOffSet = m_items.Count - m_openAreaItemsCount;
                m_scrollBarEndPositionAbs = (GetOpenedArea().LeftTop + GetOpenedArea().Size).Y;
            }
        }

        void CalculateStartAndEndDisplayItemsIndex()
        {
            m_Ratio = (m_scrollBarCurrentPosition.Value / m_maxScrollBarPosition) * (m_scrollBarItemOffSet);
            m_displayItemsStartIndex = Math.Max(0, (int)m_Ratio);
            m_displayItemsStartIndex = Math.Min(m_scrollBarItemOffSet, m_displayItemsStartIndex);
            m_displayItemsEndIndex = Math.Min(m_items.Count, m_displayItemsStartIndex + m_openAreaItemsCount + 1);
        }

        void ScrollToPreSelectedItem()
        {
            if (m_preselectedKeyboardIndex.HasValue == true)
            {
                m_displayItemsStartIndex = m_preselectedKeyboardIndex.Value <= DISPLAY_ITEM_MIDDLE_INDEX ?
                    0 : m_preselectedKeyboardIndex.Value - DISPLAY_ITEM_MIDDLE_INDEX;

                m_displayItemsEndIndex = m_displayItemsStartIndex + m_openAreaItemsCount;

                if (m_displayItemsEndIndex > m_items.Count)
                {
                    m_displayItemsEndIndex = m_items.Count;
                    m_displayItemsStartIndex = m_displayItemsEndIndex - m_openAreaItemsCount;
                }
                m_scrollBarCurrentPosition = m_displayItemsStartIndex * m_maxScrollBarPosition / m_scrollBarItemOffSet;
            }
        }

        void SetScrollBarPosition(float value)
        {
            value = MathHelper.Clamp(value, m_minScrollBarPosition, m_maxScrollBarPosition);

            if ((m_scrollBarCurrentPosition.HasValue == false) || (m_scrollBarCurrentPosition.Value != value))
            {
                m_scrollBarCurrentPosition = value;
                CalculateStartAndEndDisplayItemsIndex();
            }
        }
    }
}
