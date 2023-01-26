from django.urls import path

from . import views

urlpatterns = [
    path('electify', views.newPageView, name='home'),
]
