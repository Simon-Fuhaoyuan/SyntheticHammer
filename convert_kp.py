import os
from tqdm import tqdm

PREFIX = 'Hammer_'

KP_ROOT = './SyntheticHammer/data/hammers_kp'
KP_SUFFIX = '_kp.txt'

def convert_keypoints():
    for i in tqdm(range(2000)):
        kp_file = os.path.join(KP_ROOT, PREFIX + str(i) + KP_SUFFIX)
        kp_contents = []
        with open(kp_file) as f:
            kp_contents = f.readlines()
        f.close()

        # Ignore the first line, which is comment
        kp_contents = kp_contents[1:]
        points = []

        for j, (kp_content) in enumerate(kp_contents):
            point = kp_content.split(' ')
            points.append(point)
        
        x_abs_value = float(points[1][0])
        points[0][0] = str(float(points[0][0]) - x_abs_value)
        with open(kp_file, 'w') as f:
            f.write('Generated keypoints for hammer_{}.obj\n'.format(i))
            f.write('{} {} {}\n'.format(points[0][0], 0, 0))
            f.write('{} {} {}\n'.format(0, points[1][1], 0))
            f.write('{} {} {}\n'.format(0, points[2][1], 0))
        f.close()

if __name__ == '__main__':
    # print('{} {} {}\n'.format(0, 0, 0))
    convert_keypoints()