using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OtomatSearching.Engine
{
    public class KMPSearch
    {
        private int[] m_int_table;

        private void CreateTable(string ip_str_pattern)
        {
            var plen = ip_str_pattern.Length;
            var v_int_pos = 2;
            var v_int_cnd = 0;
            // Default assign
            this.m_int_table = new int[plen];
            this.m_int_table[0] = -1;
            this.m_int_table[1] = 0;
            while (v_int_pos < plen)
            {
                if (ip_str_pattern[v_int_pos - 1] == ip_str_pattern[v_int_cnd])
                {
                    this.m_int_table[v_int_pos] = v_int_cnd + 1;
                    v_int_pos++;
                    v_int_cnd++;
                }
                else if (v_int_cnd > 0)
                {
                    v_int_cnd = this.m_int_table[v_int_cnd];
                }
                else
                {
                    this.m_int_table[v_int_pos] = 0;
                    v_int_pos++;
                }
            }
        }

        public int Search(string ip_str_corpus, string ip_str_pattern)
        {
            var m = 0;
            var i = 0;
            var slen = ip_str_corpus.Length;
            var plen = ip_str_pattern.Length;

            this.CreateTable(ip_str_pattern);
            while (m + i < slen)
            {
                if (ip_str_pattern[i] == ip_str_corpus[m + i])
                {
                    if (++i == plen)
                    {
                        // Tìm thấy
                        return m;
                    }
                }
                else
                {
                    m += i - this.m_int_table[i];
                    if (this.m_int_table[i] > -1)
                    {
                        i = this.m_int_table[i];
                    }
                    else
                    {
                        i = 0;
                    }
                }
            }
            // Không tìm thấy
            return -1;
        }
    }
}
