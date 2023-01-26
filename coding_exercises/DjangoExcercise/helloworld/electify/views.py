from django.shortcuts import render

from django.http import HttpResponse

# Create your views here.

def newPageView(request):
    return HttpResponse('Take control of your district.')
