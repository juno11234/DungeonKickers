using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeSort : MonoBehaviour
{
    public List<int> myList = new List<int>() { 12, 11, 13, 5, 6, 7 };
    public int[] myArray = { 12, 15, 11, 5, 9, 3 };
    public int[] aux;

    private void Start()
    {
        Debug.Log("���� ��: " + string.Join(", ", myList));
        Debug.Log("���� ��: " + string.Join(", ", myArray));

        // ���� ����
        myList = SortList(myList);
        SortArray(myArray);

        Debug.Log("���� ��: " + string.Join(", ", myList));
        Debug.Log("���� ��: " + string.Join(", ", myArray));
    }

    List<int> SortList(List<int> list)
    {
        if (list.Count <= 1)
        {
            return list;
        }

        int mid = list.Count / 2;
        List<int> left = list.GetRange(0, mid);
        List<int> right = list.GetRange(mid, list.Count - mid);

        left = SortList(left);
        right = SortList(right);

        return MergeList(left, right);
    }

    List<int> MergeList(List<int> left, List<int> right)
    {
        List<int> result = new List<int>();
        int i = 0, j = 0;

        // ����, ���� �ϳ��� ī��Ʈ�� �Ѿ�� ��������
        while (i < left.Count && j < right.Count)
        {
            //���ʰ� �������� ���ؼ� �������� �߰��ϰ� �ش� �κ��� �ε��� ++
            if (left[i] <= right[j])
            {
                result.Add(left[i++]);
            }
            else
            {
                result.Add(right[j++]);
            }
        }

        // ������ �����̳� ������ �ϳ��� ����ǰ� ������ �ִٸ� ������ ����
        while (i < left.Count)
        {
            result.Add(left[i++]);
        }
        while (j < right.Count)
        {
            result.Add(right[j++]);
        }

        return result;
    }

    void SortArray(int[] arr)
    {
        aux = new int[arr.Length];
        StartSort(arr, 0, arr.Length - 1);
    }
    void StartSort(int[] arr, int left, int right)
    {
        if (left >= right)
        {
            return;
        }

        int mid = (left + right) / 2;
        StartSort(arr, left, mid);
        StartSort(arr, mid + 1, right);

        Merge(arr, left, mid, right);
    }
    void Merge(int[] arr, int left, int mid, int right)
    {
        Array.Copy(arr, left, aux, left, right - left + 1);
        // 2. �ε��� ������ ����
        int L = left;      // ���� �迭 ������
        int R = mid + 1;   // ������ �迭 ������

        // 3. ���������� ���������� ����.
        for (int current = left; current <= right; current++)
        {
            if (L > mid) // �������� �߰��� ������(���� ����) ������ �迭�� ä���
            {
                arr[current] = aux[R++];
            }
            else if (R > right) // �߾�+1�� �������� ������(������ ����) ���� �迭�� ä���
            {
                arr[current] = aux[L++];
            }
            else if (aux[L] <= aux[R]) // ���� ��Ұ� �� �۰ų� ������
            {
                //�迭�� L�� �ְ� ++
                arr[current] = aux[L++];
            }
            else // ������ ��Ұ� �� ������
            {
                //�迭�� R�� �ְ� ++
                arr[current] = aux[R++];
            }
        }
    }
}
