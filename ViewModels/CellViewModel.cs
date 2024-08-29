using Caro.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caro.ViewModels
{
    public class CellViewModel : BaseViewModel
    {
        private char    _content;
        private FPoint  _position;
        private bool    _isWinningCell;
        private int     _sizeContent;
        public char Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }
        public FPoint Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged();
            }
        }

        public bool IsWinningCell
        {
            get => _isWinningCell;
            set
            {
                _isWinningCell = value;
                OnPropertyChanged();
            }
        }

        public int SizeContent
        {
            get => _sizeContent;
            set
            {
                _sizeContent = value;
                OnPropertyChanged();
            }
        }

        public CellViewModel(FPoint position, char content, int sizeContent)
        {
            _content        = content;
            _position       = position;
            _sizeContent    = sizeContent;
        }
        
    }
}
