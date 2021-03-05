import os
import tqdm
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
import numpy as np

PREFIX = 'Hammer_'

PCD_ROOT = './SyntheticHammer/data/hammers_pcd'
PCD_SUFFIX = '.pcd'
KP_ROOT = './SyntheticHammer/data/hammers_kp'
KP_SUFFIX = '_kp.txt'

for i in range(2000):
    fig = plt.figure()
    ax = Axes3D(fig)
    pcd_file = os.path.join(PCD_ROOT, PREFIX + str(i) + PCD_SUFFIX)
    kp_file = os.path.join(KP_ROOT, PREFIX + str(i) + KP_SUFFIX)

    # Scatters
    x = []
    y = []
    z = []

    x_kp = []
    y_kp = []
    z_kp = []

    # Get point cloud
    pcd_contents = []
    with open(pcd_file) as f:
        pcd_contents = f.readlines()
    f.close()
    pcd_contents = pcd_contents[11:]
    for pcd_content in pcd_contents:
        point = pcd_content.split(' ')
        x.append(float(point[0]))
        y.append(float(point[1]))
        z.append(float(point[2]))
    
    # Get keypoints
    kp_contents = []
    with open(kp_file) as f:
        kp_contents = f.readlines()
    f.close()
    kp_contents = kp_contents[1:]
    for j, (kp_content) in enumerate(kp_contents):
        point = kp_content.split(' ')
        x_kp.append(float(point[0]))
        y_kp.append(float(point[1]))
        z_kp.append(float(point[2]))

    x = np.array(x)
    y = np.array(y)
    z = np.array(z)

    x_kp = np.array(x_kp)
    y_kp = np.array(y_kp)
    z_kp = np.array(z_kp)
    # print(x_kp.shape)

    ax.scatter(x, y, z, c='blue', marker='.', s=1, label='')
    ax.scatter(x_kp[0], y_kp[0], z_kp[0], c='red', marker='*', s=50, label='grasp point')
    ax.scatter(x_kp[1], y_kp[1], z_kp[1], c='green', marker='*', s=50, label='function point')
    ax.scatter(x_kp[2], y_kp[2], z_kp[2], c='yellow', marker='*', s=50, label='effect point')
    plt.legend()
    plt.show()
    plt.close()

