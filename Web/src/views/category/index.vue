<template>
	<div class="sys-category-container">
		<splitpanes class="default-theme">
			<pane size="20">
				<CategoryTree ref="categoryTreeRef" @node-click="nodeClick" />
			</pane>
			<pane size="80" style="overflow: auto">
				<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
					<el-form :model="state.queryParams" ref="queryForm" :inline="true">
						<el-form-item label="分类名称">
							<el-input v-model="state.queryParams.name" placeholder="分类名称" clearable />
						</el-form-item>
						<el-form-item label="分类编码">
							<el-input v-model="state.queryParams.code" placeholder="分类编码" clearable />
						</el-form-item>
						<el-form-item label="分类类型">
							<g-sys-dict v-model="state.queryParams.type" code="category_type" render-as="select" filterable clearable />
						</el-form-item>
						<el-form-item>
							<el-button-group>
								<el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
								<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
							</el-button-group>
						</el-form-item>
						<el-form-item>
							<el-button type="primary" icon="ele-Plus" @click="openAddCategory" v-auth="'sysCategory:add'"> 新增 </el-button>
						</el-form-item>
					</el-form>
				</el-card>

				<el-card class="full-table" shadow="hover" style="margin-top: 5px">
					<el-table :data="state.categoryData" style="width: 100%" v-loading="state.loading" row-key="id" default-expand-all :tree-props="{ children: 'children', hasChildren: 'hasChildren' }" border>
						<el-table-column prop="name" label="分类名称" min-width="160" header-align="center" show-overflow-tooltip />
						<el-table-column prop="code" label="分类编码" align="center" show-overflow-tooltip />
						<el-table-column prop="level" label="级别" width="70" align="center" show-overflow-tooltip />
						<el-table-column prop="type" label="分类类型" align="center" show-overflow-tooltip>
							<template #default="scope">
								<g-sys-dict v-model="scope.row.type" code="category_type" />
							</template>
						</el-table-column>
						<el-table-column prop="orderNo" label="排序" width="70" align="center" show-overflow-tooltip />
						<el-table-column label="状态" width="70" align="center" show-overflow-tooltip>
							<template #default="scope">
								<g-sys-dict v-model="scope.row.status" code="StatusEnum" />
							</template>
						</el-table-column>
						<el-table-column label="修改记录" width="100" align="center" show-overflow-tooltip>
							<template #default="scope">
								<ModifyRecord :data="scope.row" />
							</template>
						</el-table-column>
						<el-table-column label="操作" width="210" fixed="right" align="center" show-overflow-tooltip>
							<template #default="scope">
								<el-button icon="ele-Edit" text type="primary" @click="openEditCategory(scope.row)" v-auth="'sysCategory:update'"> 编辑 </el-button>
								<el-button icon="ele-Delete" text type="danger" @click="delCategory(scope.row)" v-auth="'sysCategory:delete'"> 删除 </el-button>
								<el-button icon="ele-CopyDocument" text type="primary" @click="openCopyCategory(scope.row)" v-auth="'sysCategory:add'"> 复制 </el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-card>
			</pane>
		</splitpanes>

		<EditCategory ref="editCategoryRef" :title="state.editCategoryTitle" :categoryData="state.categoryTreeData" @reload="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysCategory">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { getAPI } from '/@/utils/axios-utils';
import { SysCategoryApi } from '/@/api-services/api';
import { SysCategory, UpdateCategoryInput } from '/@/api-services/models';
import CategoryTree from '/@/views/category/component/categoryTree.vue';
import EditCategory from '/@/views/category/component/editCategory.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

const editCategoryRef = ref<InstanceType<typeof EditCategory>>();
const categoryTreeRef = ref<InstanceType<typeof CategoryTree>>();
const state = reactive({
	loading: false,
	tenantList: [] as Array<any>,
	categoryData: [] as Array<SysCategory>, // 分类列表数据
	categoryTreeData: [] as Array<SysCategory>, // 分类树所有数据
	queryParams: {
		id: 0,
		name: undefined,
		code: undefined,
		type: undefined,
	},
	tenantId: undefined,
	editCategoryTitle: '',
});

onMounted(async () => {
	handleQuery();
});

// 查询操作
const handleQuery = async (updateTree: boolean = false) => {
	state.loading = true;
	let res = await getAPI(SysCategoryApi).apiSysCategoryListGet(state.queryParams.id, state.queryParams.name, state.queryParams.code, state.queryParams.type);
	state.categoryData = res.data.result ?? [];
	state.loading = false;
	// 是否更新左侧分类列表树
	if (updateTree) {
		categoryTreeRef.value?.initTreeData();
		// 更新编辑页面分类列表树
		res = await getAPI(SysCategoryApi).apiSysCategoryListGet(0);
		state.categoryTreeData = res.data.result ?? [];
	}

	// 若无选择节点并且查询条件为空时，更新编辑页面分类列表树
	if (state.queryParams.id == 0 && state.queryParams.name == undefined && state.queryParams.code == undefined && state.queryParams.type == undefined && !updateTree)
		state.categoryTreeData = state.categoryData;
};

// 重置操作
const resetQuery = () => {
	state.queryParams.id = 0;
	state.queryParams.name = undefined;
	state.queryParams.code = undefined;
	state.queryParams.type = undefined;
	handleQuery();
};

// 打开新增页面
const openAddCategory = () => {
	state.editCategoryTitle = '添加分类';
	editCategoryRef.value?.openDialog({ status: 1, orderNo: 100, tenantId: state.tenantId });
};

// 打开编辑页面
const openEditCategory = (row: any) => {
	state.editCategoryTitle = '编辑分类';
	editCategoryRef.value?.openDialog(row);
};

// 打开复制页面
const openCopyCategory = (row: any) => {
	state.editCategoryTitle = '复制分类';
	var copyRow = JSON.parse(JSON.stringify(row)) as UpdateCategoryInput;
	copyRow.id = 0;
	copyRow.name = '';
	editCategoryRef.value?.openDialog(copyRow);
};

// 删除
const delCategory = (row: any) => {
	ElMessageBox.confirm(`确定删除分类：【${row.name}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysCategoryApi).apiSysCategoryDeletePost({ id: row.id });
			ElMessage.success('删除成功');
			handleQuery(true);
		})
		.catch(() => {});
};

// 树组件点击
const nodeClick = async (node: any) => {
	state.queryParams.id = node.id;
	state.queryParams.name = undefined;
	state.queryParams.code = undefined;
	state.queryParams.type = undefined;
	state.tenantId = node.tenantId;
	console.log(node);
	handleQuery();
};
</script>
