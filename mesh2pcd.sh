#!/bin/bash

# Convert mesh to point cloud by pcl_mesh2pcd

HAMMER="Hammer"
OBJ_DIR="hammers_obj/"
PCD_DIR="hammers_pcd/"

for i in $( seq 1 2000 )
do
	CURRENT_HAMMER=${HAMMER}_${i}
	pcl_mesh2pcd ${OBJ_DIR}$CURRENT_HAMMER.obj ${PCD_DIR}$CURRENT_HAMMER.pcd -no_vis_result

done
