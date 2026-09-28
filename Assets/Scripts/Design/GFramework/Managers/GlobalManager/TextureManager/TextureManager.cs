using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using GFramework.Core;

namespace GFramework
{
    public sealed class TextureManager : ITextureManager
    {
        // ===============================================
        // MARK: - ● Property ─────

        private Dictionary<string, Texture2D> _textureList = new Dictionary<string, Texture2D>();


        // ===============================================
        // MARK: - ● Constructor ─────

        public TextureManager()
        {
            // do nothing
        }


        // ===============================================
        // MARK: - ● Load Texture ─────

        public Texture2D LoadTexture(string filePathUnderResources, bool willCacheIfNeeded)
        {
            if (HasDataInCache(filePathUnderResources: filePathUnderResources))
            {
                return _textureList[filePathUnderResources];
            }
            else
            {
                // デザイン開発環境用
                //string fullFilePath = ConvertToFullFilePath(filePathUnderResources: filePathUnderResources);
                //byte[] bytes = File.ReadAllBytes(fullFilePath);

                // 本番環境用
                byte[] bytes = BetterStreamingAssets.ReadAllBytes(filePathUnderResources);

                Texture2D resultTexture = new Texture2D(0, 0);
                resultTexture.LoadImage(bytes);


                if (willCacheIfNeeded)
                {
                    _textureList.Add(key: filePathUnderResources, value: resultTexture);
                }


                return resultTexture;
            }
        }


        private string ConvertToFullFilePath(string filePathUnderResources)
        {
            return Path.Combine(Application.streamingAssetsPath, filePathUnderResources);
        }


        private bool HasDataInCache(string filePathUnderResources)
        {
            return _textureList.ContainsKey(filePathUnderResources);
        }


        public void PreLoadTexture(List<string> filePathUnderResourcesList)
        {
            foreach (var filePath in filePathUnderResourcesList)
            {
                LoadTexture(filePathUnderResources: filePath, willCacheIfNeeded: true);
            }
        }


        // ===============================================
        // MARK: - ● Clear Texture ─────

        public void ClearTexture()
        {
            _textureList.Clear();
        }


        // ===============================================
        // MARK: - ● Get File Path ─────

        public string GetFilePath(EWindowType windowType)
        {
            return windowType switch
            {
                EWindowType.TOP => "UI/Window/window_d1_top.png",
                EWindowType.NORMAL => "UI/Window/window_d1_normal.png",
                EWindowType.CONFIRMATION => "UI/Window/window_d1_confirmation.png",
                EWindowType.ERROR => "UI/Window/window_d1_error.png",
                _ => throw new NotImplementedException(),
            };
        }


        public string GetFilePath(EButtonImageType buttonImageType, EButtonStatusType buttonStatusType)
        {
            if (buttonImageType == EButtonImageType.BUTTON_RESIZABLE_DEFAULT ||
                buttonImageType == EButtonImageType.BUTTON_RESIZABLE_POSITIVE ||
                buttonImageType == EButtonImageType.BUTTON_RESIZABLE_DEFAULT ||
                buttonImageType == EButtonImageType.BUTTON_RESIZABLE_WARNING)
            {
                var filePathBase = "UI/Button/button_d1";

                string filePathImageType = buttonImageType switch
                {
                    EButtonImageType.BUTTON_RESIZABLE_DEFAULT => "default",
                    EButtonImageType.BUTTON_RESIZABLE_POSITIVE => "positive",
                    EButtonImageType.BUTTON_RESIZABLE_WARNING => "warning",
                    _ => throw new InvalidOperationException(),
                };


                string filePathDisplayType = buttonStatusType switch
                {
                    EButtonStatusType.DEFAULT => "default",
                    EButtonStatusType.PRESSED => "pressed",
                    EButtonStatusType.HOVER => "pressed",
                    EButtonStatusType.DISABLED => "disabled",
                    _ => throw new InvalidOperationException(),
                };

                return filePathBase + "_" + filePathImageType + "_" + filePathDisplayType + ".png";
            }


            if (buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_PROMOTION_SELECTOR ||
                buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_RESIGN ||
                buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_RESIGN_SELECTOR_CANSEL ||
                buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_RESIGN_SELECTOR_OK ||
                buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_WINDOW_CLOSE)
            {
                string filePathBase = buttonImageType switch
                {
                    EButtonImageType.BUTTON_UNRESIZABLE_PROMOTION_SELECTOR => "Stage/GameStage/PromotionSelector/promotion_selector_button",
                    EButtonImageType.BUTTON_UNRESIZABLE_RESIGN => "Stage/GameStage/button_d1_resign",
                    EButtonImageType.BUTTON_UNRESIZABLE_RESIGN_SELECTOR_CANSEL => "Stage/GameStage/ResignSelector/resign_selector_button_cansel",
                    EButtonImageType.BUTTON_UNRESIZABLE_RESIGN_SELECTOR_OK => "Stage/GameStage/ResignSelector/resign_selector_button_ok",
                    EButtonImageType.BUTTON_UNRESIZABLE_WINDOW_CLOSE => "UI/Button/button_d1_window_close",
                    _ => throw new NotImplementedException(),
                };


                string filePathDisplayType = buttonStatusType switch
                {
                    EButtonStatusType.DEFAULT => "default",
                    EButtonStatusType.PRESSED => "pressed",
                    EButtonStatusType.HOVER => "pressed",
                    EButtonStatusType.DISABLED => "disabled",
                    _ => throw new InvalidOperationException(),
                };

                return filePathBase + "_" + filePathDisplayType + ".png";
            }


            if (buttonImageType == EButtonImageType.INPUTFIELD_DEFAULT)
            {
                var filePathBase = "UI/InputField/inputfield_d1";

                string filePathImageType = buttonImageType switch
                {
                    EButtonImageType.INPUTFIELD_DEFAULT => "default",
                    _ => throw new InvalidOperationException(),
                };


                string filePathDisplayType = buttonStatusType switch
                {
                    EButtonStatusType.DEFAULT => "default",
                    EButtonStatusType.PRESSED => "pressed",
                    EButtonStatusType.HOVER => "pressed",
                    EButtonStatusType.DISABLED => "disabled",
                    _ => throw new InvalidOperationException(),
                };

                return filePathBase + "_" + filePathImageType + "_" + filePathDisplayType + ".png";
            }


            throw new InvalidOperationException();
        }


        public string GetFilePathOfInputFieldImage(EButtonStatusType buttonStatusType, bool hasError)
        {
            var filePathBase = "UI/InputField/inputfield_d1_default";

            if (hasError)
            {
                if (buttonStatusType == EButtonStatusType.HOVER)
                {
                    return filePathBase + "_" + "error_highlighted" + ".png";
                }
                else
                {
                    return filePathBase + "_" + "error_default" + ".png";
                }
            }

            string filePathDisplayType = buttonStatusType switch
            {
                EButtonStatusType.DEFAULT => "default",
                EButtonStatusType.PRESSED => "pressed",
                EButtonStatusType.HOVER => "highlighted",
                EButtonStatusType.DISABLED => "disabled",
                _ => throw new InvalidOperationException(),
            };

            return filePathBase + "_" + filePathDisplayType + ".png";
        }


        public string GetFilePathOfTextBalloonBase()
        {
            return "UI/TextBalloon/textBalloonBase.png";
        }


        public string GetFilePath(ETextureTypeForDummy textureType)
        {
            return textureType switch
            {
                ETextureTypeForDummy.AREA => "Debug/dummy_area.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForBackground textureType)
        {
            return textureType switch
            {
                ETextureTypeForBackground.BODY_A => "Stage/Common/Background/d0/background_a.png",
                ETextureTypeForBackground.BODY_B => "Stage/Common/Background/d0/background_b.png",
                ETextureTypeForBackground.SCREEN_EDGE => "Stage/Common/Background/d0/screen_edge.png",
                ETextureTypeForBackground.RIPPLE_A => "Stage/Common/Background/d0/ripple_a.png",
                ETextureTypeForBackground.RIPPLE_B => "Stage/Common/Background/d0/ripple_b.png",
                ETextureTypeForBackground.RIPPLE_C => "Stage/Common/Background/d0/ripple_c.png",
                ETextureTypeForBackground.RIPPLE_D => "Stage/Common/Background/d0/ripple_d.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForCurtain textureType)
        {
            return textureType switch
            {
                ETextureTypeForCurtain.WHITE => "UI/Window/window_background_white.png",
                ETextureTypeForCurtain.BLACK => "UI/Window/window_background_black.png",
                ETextureTypeForCurtain.BLUE => "UI/Window/window_background_blue.png",
                ETextureTypeForCurtain.RED => "UI/Window/window_background_red.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForStatusBar textureType)
        {
            return textureType switch
            {
                ETextureTypeForStatusBar.BODY => "Stage/Common/StatusBar/status_bar_body.png",
                ETextureTypeForStatusBar.LIFE_BODY => "Stage/Common/StatusBar/status_bar_life_d2_on.png",
                ETextureTypeForStatusBar.HOLE => "Stage/Common/StatusBar/status_bar_life_d2_off.png",
                ETextureTypeForStatusBar.EMBLEM_PREMIUM => "Stage/Common/StatusBar/status_bar_emblem_premium.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForTitleLogo textureType)
        {
            return textureType switch
            {
                ETextureTypeForTitleLogo.BODY => "Stage/MenuStage/title_logo.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(EBoardType boardType, ETextureTypeForBoard textureType)
        {
            if (boardType == EBoardType.BOARD_66)
            {
                return textureType switch
                {
                    ETextureTypeForBoard.BODY => "Stage/Common/BoardSet/Board/board_66_d1_body.png",
                    ETextureTypeForBoard.BODY_NUM_BLACK => "Stage/Common/BoardSet/Board/board_66_d1_body_num_black.png",
                    ETextureTypeForBoard.BODY_NUM_WHITE => "Stage/Common/BoardSet/Board/board_66_d1_body_num_white.png",
                    ETextureTypeForBoard.SHADOW => "Stage/Common/BoardSet/Board/board_d1_shadow.png",
                    _ => throw new InvalidOperationException(),
                };
            }
            else
            {
                // boardType == EBoardType.NORMAL_SHOGI
                return textureType switch
                {
                    ETextureTypeForBoard.BODY => "Stage/Common/BoardSet/Board/board_99_d1_body.png",
                    ETextureTypeForBoard.BODY_NUM_BLACK => "Stage/Common/BoardSet/Board/board_99_d1_body_num_black.png",
                    ETextureTypeForBoard.BODY_NUM_WHITE => "Stage/Common/BoardSet/Board/board_99_d1_body_num_white.png",
                    ETextureTypeForBoard.SHADOW => "Stage/Common/BoardSet/Board/board_d1_shadow.png",
                    _ => throw new InvalidOperationException(),
                }; 
            }
        }


        public string GetFilePath(ETextureTypeForWaitingSpace textureType)
        {
            return textureType switch
            {
                ETextureTypeForWaitingSpace.BODY_HORIZONTAL_P1 => "Stage/Common/BoardSet/WaitingSpace/waiting_space_d1_body_horizontal_p1.png",
                ETextureTypeForWaitingSpace.BODY_HORIZONTAL_P2 => "Stage/Common/BoardSet/WaitingSpace/waiting_space_d1_body_horizontal_p2.png",
                ETextureTypeForWaitingSpace.BODY_SQUARE_P1 => "Stage/Common/BoardSet/WaitingSpace/waiting_space_d1_body_square_p1.png",
                ETextureTypeForWaitingSpace.BODY_SQUARE_P2 => "Stage/Common/BoardSet/WaitingSpace/waiting_space_d1_body_square_p2.png",
                ETextureTypeForWaitingSpace.SHADOW_HORIZONTAL => "Stage/Common/BoardSet/WaitingSpace/waiting_space_d1_shadow_horizontal.png",
                ETextureTypeForWaitingSpace.SHADOW_SQUARE => "Stage/Common/BoardSet/WaitingSpace/waiting_space_d1_shadow_square.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForPromotionSelector textureType)
        {
            return textureType switch
            {
                ETextureTypeForPromotionSelector.BODY => "Stage/GameStage/PromotionSelector/promotion_selector_body.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForResignSelector textureType)
        {
            return textureType switch
            {
                ETextureTypeForResignSelector.BODY => "Stage/GameStage/ResignSelector/resign_selector_body.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForUISlider textureType)
        {
            return textureType switch
            {
                ETextureTypeForUISlider.BASE_DEFAULT => "UI/Slider/slider_d1_base_default.png",
                ETextureTypeForUISlider.BASE_DISABLED => "UI/Slider/slider_d1_base_disabled.png",
                ETextureTypeForUISlider.BAR_DEFAULT => "UI/Slider/slider_d1_bar_default.png",
                ETextureTypeForUISlider.BAR_DISABLED => "UI/Slider/slider_d1_bar_disabled.png",
                ETextureTypeForUISlider.MARK_DEFAULT => "UI/Slider/slider_d1_mark_default.png",
                ETextureTypeForUISlider.MARK_DISABLED => "UI/Slider/slider_d1_mark_disabled.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForUICheckBox textureType)
        {
            return textureType switch
            {
                ETextureTypeForUICheckBox.BASE_DEFAULT => "UI/CheckBox/checkbox_d1_base_default.png",
                ETextureTypeForUICheckBox.BASE_HIGHLIGHTED => "UI/CheckBox/checkbox_d1_base_highlighted.png",
                ETextureTypeForUICheckBox.BASE_ERROR => "UI/CheckBox/checkbox_d1_base_error.png",
                ETextureTypeForUICheckBox.BASE_DISABLED => "UI/CheckBox/checkbox_d1_base_disabled.png",
                ETextureTypeForUICheckBox.MARK_ON_DEFAULT => "UI/CheckBox/checkbox_d1_mark_on_default.png",
                ETextureTypeForUICheckBox.MARK_ON_DISABLED => "UI/CheckBox/checkbox_d1_mark_on_disabled.png",
                ETextureTypeForUICheckBox.MARK_OFF => "UI/CheckBox/checkbox_d1_mark_off.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForUIRadioButton textureType)
        {
            return textureType switch
            {
                ETextureTypeForUIRadioButton.BASE_DEFAULT => "UI/RadioButton/radiobutton_d1_base_default.png",
                ETextureTypeForUIRadioButton.BASE_HIGHLIGHTED => "UI/RadioButton/radiobutton_d1_base_highlighted.png",
                ETextureTypeForUIRadioButton.BASE_ERROR => "UI/RadioButton/radiobutton_d1_base_error.png",
                ETextureTypeForUIRadioButton.BASE_DISABLED => "UI/RadioButton/radiobutton_d1_base_disabled.png",
                ETextureTypeForUIRadioButton.MARK_ON_DEFAULT => "UI/RadioButton/radiobutton_d1_mark_on_default.png",
                ETextureTypeForUIRadioButton.MARK_ON_DISABLED => "UI/RadioButton/radiobutton_d1_mark_on_disabled.png",
                ETextureTypeForUIRadioButton.MARK_OFF => "UI/RadioButton/radiobutton_d1_mark_off.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForButtonIcon textureType)
        {
            return textureType switch
            {
                ETextureTypeForButtonIcon.NEXT => "UI/ButtonIcon/button_icon_next.png",
                ETextureTypeForButtonIcon.PREVIOUS => "UI/ButtonIcon/button_icon_previous.png",
                ETextureTypeForButtonIcon.TO_FIRST => "UI/ButtonIcon/button_icon_to_first.png",
                ETextureTypeForButtonIcon.TO_LAST => "UI/ButtonIcon/button_icon_to_last.png",
                ETextureTypeForButtonIcon.SETTING => "UI/ButtonIcon/button_icon_setting.png",
                ETextureTypeForButtonIcon.PLAY_RANSHOGI => "UI/ButtonIcon/button_icon_play_ranshogi.png",
                ETextureTypeForButtonIcon.PLAY_SHOGI => "UI/ButtonIcon/button_icon_play_shogi.png",
                ETextureTypeForButtonIcon.PLAY_WITH_FRIEND => "UI/ButtonIcon/button_icon_play_with_friend.png",
                
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePath(ETextureTypeForMenuStage textureType)
        {
            return textureType switch
            {
                ETextureTypeForMenuStage.BUTTON_DEFAULT => "UI/Button/button_d1_default_default.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePathOfImageText(int designID, char character)
        {
            string folderNameRoot = "UI/ImageText";
            string nameDesignID = "d" + designID.ToString();
            string fileName = character switch
            {
                ' ' => "image_text_" + nameDesignID + "_blank.png",
                '0' => "image_text_" + nameDesignID + "_0.png",
                '1' => "image_text_" + nameDesignID + "_1.png",
                '2' => "image_text_" + nameDesignID + "_2.png",
                '3' => "image_text_" + nameDesignID + "_3.png",
                '4' => "image_text_" + nameDesignID + "_4.png",
                '5' => "image_text_" + nameDesignID + "_5.png",
                '6' => "image_text_" + nameDesignID + "_6.png",
                '7' => "image_text_" + nameDesignID + "_7.png",
                '8' => "image_text_" + nameDesignID + "_8.png",
                '9' => "image_text_" + nameDesignID + "_9.png",
                '+' => "image_text_" + nameDesignID + "_plus.png",
                '-' => "image_text_" + nameDesignID + "_minus.png",
                ':' => "image_text_" + nameDesignID + "_colon.png",
                _ => "image_text_" + nameDesignID + "_null.png",
            };
            return folderNameRoot + "/" + nameDesignID + "/" + fileName;
        }

        public string GetFilePathOfGradeRankIcon(EGradeRankType gradeRankType, EBoardType boardType, bool isDefault)
        {
            string a = "Stage/Common/GradeRankIcon/grade_rank_icon";

            string b = boardType switch
            {
                EBoardType.BOARD_66 => "66",
                EBoardType.BOARD_99 => "99",
                _ => throw new InvalidOperationException(),
            };
            
            string c = gradeRankType switch
            {
                EGradeRankType.NONE => "none",
                EGradeRankType.GRADE_15 => "grade_15",
                EGradeRankType.GRADE_14 => "grade_14",
                EGradeRankType.GRADE_13 => "grade_13",
                EGradeRankType.GRADE_12 => "grade_12",
                EGradeRankType.GRADE_11 => "grade_11",
                EGradeRankType.GRADE_10 => "grade_10",
                EGradeRankType.GRADE_9 => "grade_9",
                EGradeRankType.GRADE_8 => "grade_8",
                EGradeRankType.GRADE_7 => "grade_7",
                EGradeRankType.GRADE_6 => "grade_6",
                EGradeRankType.GRADE_5 => "grade_5",
                EGradeRankType.GRADE_4 => "grade_4",
                EGradeRankType.GRADE_3 => "grade_3",
                EGradeRankType.GRADE_2 => "grade_2",
                EGradeRankType.GRADE_1 => "grade_1",
                EGradeRankType.RANK_1 => "rank_1",
                EGradeRankType.RANK_2 => "rank_2",
                EGradeRankType.RANK_3 => "rank_3",
                EGradeRankType.RANK_4 => "rank_4",
                EGradeRankType.RANK_5 => "rank_5",
                EGradeRankType.RANK_6 => "rank_6",
                EGradeRankType.RANK_7 => "rank_7",
                EGradeRankType.RANK_8 => "rank_8",
                EGradeRankType.RANK_9 => "rank_9",
                EGradeRankType.RANK_10 => "rank_10",
                EGradeRankType.AI => "ai",
                _ => throw new InvalidOperationException(),
            };

            string d = isDefault ? "default" : "on";

            return a + "_" + b + "_" + c + "_" + d + ".png";
        }

        public string GetFilePathOfGradeRankCharacter(EGradeRankType gradeRankType)
        {
            if (gradeRankType.IsGrade())
            {
                return "Stage/MypageStage/mypage_character_grade.png";
            }
            else
            {
                return "Stage/MypageStage/mypage_character_rank.png";
            }   
        }


        public string GetFilePath(EGameResultType gameResultType)
        {
            return gameResultType switch
            {
                EGameResultType.WIN => "Stage/GameStage/GameResult/game_result_main_image_win.png",
                EGameResultType.LOSE => "Stage/GameStage/GameResult/game_result_main_image_lose.png",
                EGameResultType.DRAW => "Stage/GameStage/GameResult/game_result_main_image_draw.png",
                _ => throw new InvalidOperationException(),
            };
        }


        public string GetFilePathOfGameResultConsecutiveCount(EGameResultType gameResultType, int count)
        {
            string baseString = "Stage/GameStage/GameResult/game_result_consecutive";

            var gameResultTypeString = gameResultType switch
            {
                EGameResultType.WIN => "win",
                EGameResultType.LOSE => "lose",
                EGameResultType.DRAW => "draw",
                _ => throw new InvalidOperationException(),
            };

            if (count <= 5)
            {
                return baseString + "_" + gameResultTypeString + "_" + count.ToString() + ".png";
            }
            else
            {
                return baseString + "_" + gameResultTypeString + "_" + "over" + ".png";
            }
        }


        public string GetFilePathOfGameResultRateBaseImage(EBoardType boardType)
        {
            if (boardType == EBoardType.BOARD_66)
            {
                return "Stage/GameStage/GameResult/game_result_rate_base_66.png";
            }
            else
            {
                // boardType == EBoardType.BOARD_99
                return "Stage/GameStage/GameResult/game_result_rate_base_99.png";
            }
        }


        public string GetFilePathOfGameResultRateArrowImage(GameResultRateInfoBehaviour.EGameResultRateArrowType gameResultRateArrowType)
        {
            string baseString = "Stage/GameStage/GameResult/game_result_rate_";

            string element = gameResultRateArrowType switch
            {
                GameResultRateInfoBehaviour.EGameResultRateArrowType.UP => "up",
                GameResultRateInfoBehaviour.EGameResultRateArrowType.DOWN => "down",
                GameResultRateInfoBehaviour.EGameResultRateArrowType.EVEN => "even",
                _ => throw new InvalidOperationException(),
            };

            return baseString + element + ".png";
        }


        public string GetFilePath(EHistoryItemResult historyItemResult)
        {
            string baseString = "Stage/HistoryStage/history_result_icon_";

            string element = historyItemResult switch
            {
                EHistoryItemResult.WIN => "win",
                EHistoryItemResult.LOSE => "lose",
                EHistoryItemResult.DRAW => "draw",
                EHistoryItemResult.UNKNOWN => "unknown",
                _ => throw new InvalidOperationException(),
            };

            return baseString + element + ".png";
        }

        // 対局履歴の盤アイコン
        public string GetFilePath(EBoardType boardType)
        {
            return boardType switch
            {
                EBoardType.BOARD_66 => "Stage/HistoryStage/history_board_type_icon_66.png",
                EBoardType.BOARD_99 => "Stage/HistoryStage/history_board_type_icon_99.png",
                _ => throw new InvalidOperationException(),
            };
        }




        // ===============================================
        // MARK: - ● Destory ─────

        public void Destroy()
        {
            ClearTexture();
        }
    }
}