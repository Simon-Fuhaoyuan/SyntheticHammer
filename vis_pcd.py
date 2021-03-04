import open3d as o3d
import os

pcd_dir = 'hammers_pcd'
pcd_files = os.listdir(pcd_dir)
for f in pcd_files:
    filename = os.path.join(pcd_dir, f)
    pcd = o3d.io.read_point_cloud(filename)
    o3d.visualization.draw_geometries([pcd])
