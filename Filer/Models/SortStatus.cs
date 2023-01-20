namespace Filer.Models
{
    public class SortStatus
    {
        public enum SortKey
        {
            /// <summary>
            /// ファイル名を表す
            /// </summary>
            Name,

            /// <summary>
            /// 最終更新日時を表す
            /// </summary>
            Updated,

            /// <summary>
            /// 作成日時を表す
            /// </summary>
            Created,

            /// <summary>
            /// 拡張子を表す
            /// </summary>
            Extension,
        }

        public SortKey Key { get; set; } = SortKey.Name;

        public bool Reverse { get; set; }
    }
}