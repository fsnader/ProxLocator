# -*- coding: utf-8 -*-
#============================================================================#
# Importing the libraries
import numpy as np
import matplotlib.pyplot as plt
import pandas as pd
from keras.models import Sequential
from keras.layers import Dense
from sklearn.datasets import make_regression
from sklearn.preprocessing import MinMaxScaler
import KerasModeltoJSON as js

#============================================================================#
# Rede Neural de Classificação
#============================================================================#
dataset_entrada_dist = pd.read_csv('ENTRADA_DIST.csv')
dataset_saida_dist = pd.read_csv('SAIDA_DIST.csv')
X_dist = dataset_entrada_dist.iloc[:, 0:2].values
y_dist = dataset_saida_dist.iloc[:, 0:1].values

nn_dist = Sequential()
nn_dist.add(Dense(2, input_dim=2, activation='relu'))
nn_dist.add(Dense(26, activation='relu'))
nn_dist.add(Dense(1, activation='linear'))
nn_dist.compile(loss='mse', optimizer='adam')
nn_dist.fit(X_dist, y_dist, epochs=1000, verbose=0)

wrt = js.JSONwriter(nn_dist, 'nn_dist.json')
wrt.save()
#============================================================================#
# Rede Neural 1 - Distancias Menores ou iguais a 3.5m
#============================================================================#
dataset_entrada_1 = pd.read_csv('ENTRADA_NN_1.csv')
dataset_saida_1 = pd.read_csv('SAIDA_NN_1.csv')
X_1 = dataset_entrada_1.iloc[:, 0:7].values
y_1 = dataset_saida_1.iloc[:, 0:2].values

nn_1 = Sequential()
nn_1.add(Dense(6, input_dim=6, activation='relu'))
nn_1.add(Dense(16, activation='relu'))
nn_1.add(Dense(2, activation='linear'))
nn_1.compile(loss='mse', optimizer='adam')
nn_1.fit(X_1, y_1, epochs=1000, verbose=0)

wrt = js.JSONwriter(nn_1, 'nn_1.json')
wrt.save()
#============================================================================#
# Rede Neural 2 - Para posicoes incertas
#============================================================================#
dataset_entrada_2 = pd.read_csv('ENTRADA_NN_2.csv')
dataset_saida_2 = pd.read_csv('SAIDA_NN_2.csv')
X_2 = dataset_entrada_2.iloc[:, 0:7].values
y_2 = dataset_saida_2.iloc[:, 0:2].values

nn_2 = Sequential()
nn_2.add(Dense(6, input_dim=6, activation='relu'))
nn_2.add(Dense(16, activation='relu'))
nn_2.add(Dense(2, activation='linear'))
nn_2.compile(loss='mse', optimizer='adam')
nn_2.fit(X_2, y_2, epochs=1000, verbose=0)

wrt = js.JSONwriter(nn_2, 'nn_2.json')
wrt.save()
#============================================================================#
# Rede Neural 3 - Para posicoes maiores que 3.5 m
#============================================================================#
dataset_entrada_3 = pd.read_csv('ENTRADA_NN_3.csv')
dataset_saida_3 = pd.read_csv('SAIDA_NN_3.csv')
X_3 = dataset_entrada_3.iloc[:, 0:7].values
y_3 = dataset_saida_3.iloc[:, 0:2].values

nn_3 = Sequential()
nn_3.add(Dense(6, input_dim=6, activation='relu'))
nn_3.add(Dense(16, activation='relu'))
nn_3.add(Dense(2, activation='linear'))
nn_3.compile(loss='mse', optimizer='adam')
nn_3.fit(X_3, y_3, epochs=1000, verbose=0)

wrt = js.JSONwriter(nn_3, 'nn_3.json')
wrt.save()