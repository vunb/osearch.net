using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OtomatSearching.Engine
{
    public class OtomatSearch
    {
        private int[] m_int_next;
        private int m_int_violate;  // Lỗi vi phạm
        private int m_int_threshold;
        private int m_int_position;
        /// <summary>
        /// Ngưỡng phạt của Otomat
        /// </summary>
        public int Threshold
        {
            get { return m_int_threshold; }
            set { m_int_threshold = value; }
        }
        /// <summary>
        /// Lỗi phạm phải
        /// </summary>
        public int Violate
        {
            get { return m_int_violate; }
        }

        public int Position
        {
            get { return m_int_position; }
        }

        public int Length
        {
            get { return m_int_next.Length + m_int_violate; }
        }
        // Nhảy tới vị trí tìm tiếp theo trong đoạn văn bản
        public int Next
        {
            get { return m_int_position + m_int_next.Length + m_int_violate; }
        }

        public OtomatSearch()
        {
            this.m_int_threshold = 4;
            this.m_int_position = 0;    // Mặc định ban đầu là 0
            this.m_int_violate = 0;
            this.m_int_next = new int[0];
        }

        public OtomatSearch(int ip_int_threshold)
            : this()
        {
            this.m_int_threshold = ip_int_threshold;
        }

        private void CreateNextTable(string ip_str_pattern)
        {
            var plen = ip_str_pattern.Length;
            if (plen <= 1)
            {
                this.m_int_next = new int[plen];
                this.m_int_next[0] = -1;
                return;
            }
            var v_int_pos = 2;
            var v_int_cnd = 0;
            // Default assign
            this.m_int_next = new int[plen];
            this.m_int_next[0] = -1;
            this.m_int_next[1] = 0;
            while (v_int_pos < plen)
            {
                if (Char.ToUpper(ip_str_pattern[v_int_pos - 1]) == Char.ToUpper(ip_str_pattern[v_int_cnd]))
                {
                    this.m_int_next[v_int_pos] = v_int_cnd + 1;
                    v_int_pos++;
                    v_int_cnd++;
                }
                else if (v_int_cnd > 0)
                {
                    v_int_cnd = this.m_int_next[v_int_cnd];
                }
                else
                {
                    this.m_int_next[v_int_pos] = 0;
                    v_int_pos++;
                }
            }
        }
        /// <summary>
        /// Tìm kiếm theo tiếp cận Otomat
        /// </summary>
        /// <param name="ip_str_corpus">Văn bản tìm</param>
        /// <param name="ip_str_pattern">Mẫu</param>
        /// <returns></returns>
        public int Osearch(string ip_str_corpus, string ip_str_pattern, int ip_int_startIndex, bool ip_sapproximate)
        {
            if (ip_int_startIndex < 0)
            {
                throw new Exception("Index must be greater than zero");
            }
            return ip_sapproximate == true ?
                SearchApproximate(ip_str_corpus, ip_str_pattern, ip_int_startIndex) :
                Search(ip_str_corpus, ip_str_pattern, ip_int_startIndex);
        }

        public int SearchApproximate(string ip_str_corpus, string ip_str_pattern, int ip_int_startIndex)
        {
            var v_int_violate = 0;
            this.m_int_position = -1;
            this.m_int_violate = int.MaxValue;
            var plen = ip_str_pattern.Length;
            var slen = ip_str_corpus.Length;
            // Create next table
            this.CreateNextTable(ip_str_pattern);

            var k = 0;
            var j = 0;  // chỉ số trên mẫu p
            var i = 0;  // chỉ số tại th.điểm xảy ra ko khớp
            var e = 0; //   Lỗi phạm sai
            var m = ip_int_startIndex;
            var pattern = ip_str_pattern.ToUpper();
            while ((k = m + j + v_int_violate) < slen)
            {
                var sk = Char.ToUpper(ip_str_corpus[k]);
                if (sk == pattern[j])
                {
                    if (++j == plen)
                    {
                        if (v_int_violate == 0)
                        {
                            this.m_int_violate = 0;
                            this.m_int_position = m;
                            break;
                        }
                        else if (v_int_violate < this.m_int_violate)
                        {
                            this.m_int_violate = v_int_violate;
                            this.m_int_position = m;
                        }
                        // Tiếp tục tìm mới để được lỗi là nhỏ nhất
                        m += i - this.m_int_next[i];
                        j = this.m_int_next[i] > -1 ? this.m_int_next[i] : 0;
                        v_int_violate = 0; // reset
                    }
                }
                else if ((j + 1 < plen) && sk == pattern[j + 1])
                {
                    // TH này coi như ko phạm lỗi
                    if (++j == plen - 1)
                    {
                        // Lỗi phạm ít nhất 1 lỗi --> nên ko cần xét trường hợp (v_int_violate == this.m_int_violate
                        if (v_int_violate < this.m_int_violate)
                        {
                            this.m_int_violate = v_int_violate;
                            this.m_int_position = m;
                        }
                        // Tiếp tục tìm mới để được lỗi là nhỏ nhất
                        m += i - this.m_int_next[i];
                        j = this.m_int_next[i] > -1 ? this.m_int_next[i] : 0;
                        v_int_violate = 0; // reset
                    }
                }
                else
                {
                    // Mistakes or mismatches
                    v_int_violate++;
                    // Search new if violate exceeds m_int_threshold(4)
                    if (v_int_violate > m_int_threshold)
                    {
                        m += i - this.m_int_next[i];
                        j = this.m_int_next[i] > -1 ? this.m_int_next[i] : 0;
                        v_int_violate = 0;  // reset
                    }
                    else if (v_int_violate == 1)
                    {
                        i = j;
                    }
                }
            }
            // Kiểm tra lần cuối, trường hợp duyệt hết

            return this.m_int_position;
        }

        public int Search(string ip_str_corpus, string ip_str_pattern, int ip_int_startIndex)
        {
            var k = 0;
            var i = 0;
            var m = ip_int_startIndex;
            var slen = ip_str_corpus.Length;
            var plen = ip_str_pattern.Length;

            this.m_int_position = -1;
            this.m_int_violate = 0;
            this.CreateNextTable(ip_str_pattern);
            while ((k = m + i) < slen)
            {
                if (Char.ToUpper(ip_str_pattern[i]) == Char.ToUpper(ip_str_corpus[k]))
                {
                    if (++i == plen)
                    {
                        // Tìm thấy
                        this.m_int_position = m;
                        break;
                    }
                }
                else
                {
                    m += i - this.m_int_next[i];
                    i = this.m_int_next[i] > -1 ? this.m_int_next[i] : 0;
                }
            }

            return this.m_int_position;
        }

    }
}
