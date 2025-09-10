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
        Debug.Log("정렬 전: " + string.Join(", ", myList));
        Debug.Log("정렬 전: " + string.Join(", ", myArray));

        // 정렬 시작
        myList = SortList(myList);
        SortArray(myArray);

        Debug.Log("정렬 후: " + string.Join(", ", myList));
        Debug.Log("정렬 후: " + string.Join(", ", myArray));
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

        // 좌측, 우측 하나라도 카운트가 넘어가면 루프종료
        while (i < left.Count && j < right.Count)
        {
            //왼쪽과 오른쪽을 비교해서 더작은것 추가하고 해당 부분의 인덱스 ++
            if (left[i] <= right[j])
            {
                result.Add(left[i++]);
            }
            else
            {
                result.Add(right[j++]);
            }
        }

        // 위에서 좌측이나 우측중 하나가 종료되고 남은게 있다면 남은것 실행
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
        // 2. 인덱스 포인터 설정
        int L = left;      // 왼쪽 배열 시작점
        int R = mid + 1;   // 오른쪽 배열 시작점

        // 3. 시작점에서 끝지점까지 루프.
        for (int current = left; current <= right; current++)
        {
            if (L > mid) // 시작점이 중간을 넘으면(왼쪽 소진) 오른쪽 배열로 채우기
            {
                arr[current] = aux[R++];
            }
            else if (R > right) // 중앙+1이 오른쪽을 넘으면(오른쪽 소진) 왼쪽 배열로 채우기
            {
                arr[current] = aux[L++];
            }
            else if (aux[L] <= aux[R]) // 왼쪽 요소가 더 작거나 같으면
            {
                //배열에 L를 넣고 ++
                arr[current] = aux[L++];
            }
            else // 오른쪽 요소가 더 작으면
            {
                //배열에 R를 넣고 ++
                arr[current] = aux[R++];
            }
        }
    }
}
